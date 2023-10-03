import { Construct } from 'constructs';
import { Names, JsonPatch, ApiObject } from 'cdk8s';
import {
  KubeDeployment,
  KubeHorizontalPodAutoscalerV2Beta2,
  KubeService,
  Probe,
  EnvVar,
  EnvFromSource,
  IoK8SApiCoreV1ContainerImagePullPolicy,
  Quantity,
  IntOrString,
} from './k8s';
import {
  DestinationRule,
  DestinationRuleSpecTrafficPolicyLoadBalancerSimple,
  Gateway,
  VirtualService,
  VirtualServiceSpecHttpMatch,
} from './networking.istio.io';
import { Canary, CanarySpecTargetRefKind, CanarySpecAutoscalerRefKind, CanarySpecAnalysis } from './flagger.app';

export interface WebServiceProps {
  /**
   * Name of the web service. Makes things easier to read.
   */
  readonly name: string;

  /**
   * Namespace to deploy the web service into. Defaults to 'default'
   */
  readonly namespace?: string;

  /**
   * The Docker image to use for this service.
   */
  readonly image: string;

  /**
   * Minimum umber of replicas.
   *
   * @default 1
   */
  readonly minReplicas?: number;

  /**
   * Maximum umber of replicas.
   *
   * @default 1
   */
  readonly maxReplicas?: number;

  /**
   * External port.
   *
   * @default 80
   */
  readonly port?: number;

  /**
   * Internal port.
   *
   * @default 8080
   */
  readonly containerPort?: number;

  /**
   * Arguments to the entrypoint. The docker image's CMD is used if this is not provided. Variable references $(VAR_NAME) are expanded using the container's environment. If a variable cannot be resolved, the reference in the input string will be unchanged. The $(VAR_NAME) syntax can be escaped with a double $$, ie: $$(VAR_NAME). Escaped references will never be expanded, regardless of whether the variable exists or not. Cannot be updated.
   *
   * @default []
   */
  readonly command?: string[];

  /**
   * Environmental variables to pass into docker.
   *
   * @default []
   */
  readonly env?: EnvVar[];

  /**
   * Environmental variables to pass into docker.
   *
   * @default []
   */
  readonly envFrom?: EnvFromSource[];

  /**
   * Periodic probe of container service readiness. Container will be removed from service endpoints if the probe fails. Cannot be updated. More info: https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle#container-probes
   */
  readonly readinessProbe?: Probe;

  /**
   * Periodic probe of container liveness. Container will be restarted if the probe fails. Cannot be updated. More info: https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle#container-probes
   */
  readonly livenessProbe?: Probe;

  /**
   * Labels are key/value pairs that are attached to objects, such as pods. Labels are intended to be used to specify identifying attributes of objects that are meaningful and relevant to users, but do not directly imply semantics to the core system. More info: https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/
   */
  readonly labels?: { [key: string]: string };

  /**
   * Annotations is an unstructured key value map stored with a resource that may be set by external tools to store and retrieve arbitrary metadata. They are not queryable and should be preserved when modifying objects. More info: http://kubernetes.io/docs/user-guide/annotations
   */
  readonly annotations?: { [key: string]: string };

  /**
   * Match rule for the Istio VirtualService HTTPRoute. More info: https://istio.io/latest/docs/reference/config/networking/virtual-service/#HTTPRoute
   */
  readonly match?: VirtualServiceSpecHttpMatch[];

  /**
   * Analysis spec for Flagger to determine deployment promotion conditions. More info: https://docs.flagger.app/usage/metrics
   */
  readonly canaryAnalysis: CanarySpecAnalysis;

  /**
   * Port name configuration for Flagger canary resource, set to "grpc" for gRPC services. More info: https://docs.flagger.app/usage/how-it-works#canary-service
   */
  readonly portName?: string;

  /**
   * domain name for the gateway
   */
  readonly domainName?: string;

  /**
   * Docker image pull secret auth, basically base64 encoded "username:password" for docker registry
   */
  readonly imagePullSecretName?: string;
}

export class WebService extends Construct {
  constructor(scope: Construct, id: string, props: WebServiceProps) {
    super(scope, id);

    const port = props.port || 80;
    const containerPort = props.containerPort || 8080;
    const label = { app: Names.toDnsLabel(this) };
    const labels = { ...props.labels, ...label };
    const command = props.command ?? [];
    const env = props.env;
    const envFrom = props.envFrom;
    const namespace = props.namespace ?? 'default';
    const { domainName, image, name, annotations, livenessProbe, readinessProbe, match, portName, canaryAnalysis } =
      props;
    const serviceFQDN = `${name}.${namespace}.svc.cluster.local`;

    new KubeDeployment(this, 'deployment', {
      metadata: {
        name: id,
        labels,
      },
      spec: {
        selector: {
          matchLabels: label,
        },
        template: {
          metadata: { labels, name: id, annotations },
          spec: {
            serviceAccountName: 'serviceaccount',
            containers: [
              {
                name,
                image,
                ports: [{ containerPort }],
                command,
                imagePullPolicy: IoK8SApiCoreV1ContainerImagePullPolicy.ALWAYS,
                readinessProbe,
                livenessProbe,
                env,
                envFrom,
                resources: {
                  limits: {
                    cpu: Quantity.fromNumber(1),
                    memory: Quantity.fromString(String('256Mi')),
                  },
                  requests: {
                    cpu: Quantity.fromNumber(1),
                    memory: Quantity.fromString(String('128Mi')),
                  }
                },
                securityContext: {
                  allowPrivilegeEscalation: false
                }
              },
            ],
            imagePullSecrets: [{ name: props.imagePullSecretName }],
          },
        },
      },
    });

    new KubeService(this, 'service', {
      metadata: {
        name: id,
      },
      spec: {
        ports: [{ port, targetPort: IntOrString.fromNumber(containerPort), name: `${port}-${containerPort}` }],
        selector: label,
      },
    });

    new KubeHorizontalPodAutoscalerV2Beta2(this, 'hpa', {
      metadata: {
        name: id,
      },
      spec: {
        scaleTargetRef: {
          apiVersion: 'apps/v1',
          kind: 'Deployment',
          name: id,
        },
        minReplicas: 1,
        maxReplicas: 2,
        metrics: [
          {
            type: 'Resource',
            resource: {
              name: 'cpu',
              target: {
                type: 'Utilization',
                averageUtilization: 50,
              },
            },
          },
        ],
      },
    });

    const gatewayName = this.createGateway(namespace, domainName)?.name;
    const gateways = gatewayName ? [gatewayName] : undefined;

    new VirtualService(this, 'vs', {
      spec: {
        hosts: [domainName ?? serviceFQDN],
        gateways,
        http: [
          {
            match,
            route: [
              {
                destination: {
                  host: serviceFQDN,
                },
              },
            ],
          },
        ],
        exportTo: ['*'],
      },
    });

    new DestinationRule(this, 'dr', {
      spec: {
        host: serviceFQDN,
        trafficPolicy: {
          loadBalancer: {
            simple: DestinationRuleSpecTrafficPolicyLoadBalancerSimple.LEAST_CONN,
          },
        },
        exportTo: ['*'],
      },
    });

    new Canary(this, 'canary', {
      metadata: {
        name: id,
      },
      spec: {
        targetRef: {
          apiVersion: 'apps/v1',
          kind: CanarySpecTargetRefKind.DEPLOYMENT,
          name: id,
        },
        autoscalerRef: {
          apiVersion: 'autoscaling/v2beta2',
          kind: CanarySpecAutoscalerRefKind.HORIZONTAL_POD_AUTOSCALER,
          name: id,
        },
        service: {
          port: containerPort,
          gateways,
          portName,
          canary: {
            annotations: {
              'konghq.com/override': id
            }
          },
          primary: {
            annotations: {
              'konghq.com/override': id
            }
          }
        },
        analysis: canaryAnalysis,
      },
    });
  }

  createGateway(namespace: string, domainName?: string) {
    if (!domainName) return undefined;

    const gateway = new Gateway(this, 'istio-gateway', {
      spec: {
        selector: { istio: 'ingressgateway' },
        servers: [
          {
            port: {
              number: 8080,
              name: 'http',
              protocol: 'HTTP',
            },
            hosts: [domainName],
          },
        ],
      },
    });

    this.patchResourceMetadata(gateway, {
      name: gateway.name,
      namespace,
      annotations: {
        'external-dns.alpha.kubernetes.io/hostname': domainName,
      },
    });

    return gateway;
  }

  private patchResourceMetadata(resource: Construct, metadata: any): void {
    const apiObject = ApiObject.of(resource);
    apiObject.addJsonPatch(JsonPatch.add('/metadata', metadata));
  }
}
