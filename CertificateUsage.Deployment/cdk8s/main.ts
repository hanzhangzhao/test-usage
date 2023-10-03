import { App, Chart } from 'cdk8s';
import * as config from 'config';
import { Construct } from 'constructs';
import {
  IntOrString,
  KubeJob,
  KubeNamespace,
  KubeServiceAccount,
  KubeSecret,
  IoK8SApiCoreV1ContainerImagePullPolicy,
  IoK8SApiCoreV1HttpGetActionScheme,
  IoK8SApiCoreV1PodSpecRestartPolicy,
  Quantity,
} from './imports/k8s';
import { WebService } from './imports/WebService';
import { StandaloneDeployment } from './imports/StandaloneDeployment';

const namespace = 'certificate-usage';
const registry = config.get<string>('docker.registry');
const listenerImageLabel = config.get<string>('listener.imageLabel')
const apiImageLabel = config.get<string>('api.imageLabel')
const migrationImageLabel = config.get<string>('migration.imageLabel');
const projectionsImageLabel = config.get<string>('projections.imageLabel');
const apmSecretToken = config.get<string>('apm.secretToken');
const migrationImageTag = `${registry}/certificate-usage/migrations:${migrationImageLabel}`;

const dbEnvs = [
  { name: 'DB__HOST', valueFrom: { secretKeyRef: { name: 'app-secrets', key: 'endpoint' } } },
  { name: 'DB__USERNAME', valueFrom: { secretKeyRef: { name: 'app-secrets', key: 'username' } } },
  { name: 'DB__PASSWORD', valueFrom: { secretKeyRef: { name: 'app-secrets', key: 'password' } } },
  { name: 'DB__PORT', valueFrom: { secretKeyRef: { name: 'app-secrets', key: 'port' } } },
  {
    name: 'DB__CONNECTIONSTRING',
    value: 'User ID=$(DB__USERNAME);Password=$(DB__PASSWORD);Host=$(DB__HOST);Port=$(DB__PORT);Search Path=certificate_usage',
  },
];

export class Deployment extends Chart {
  private registryToken = process.env.GITLAB_K8S_REG_TOKEN;
  private imagePullSecretName = 'regcred';

  private envFrom = [{ configMapRef: { name: 'app-config' } }];

  constructor(scope: Construct, id: string) {
    super(scope, id, { namespace });

    new KubeNamespace(this, 'namespace', {
      metadata: {
        name: namespace,
        labels: {
          'istio-injection': 'disabled',
        },
      },
    });

    new KubeServiceAccount(this, 'serviceaccount', {
      metadata: {
        name: 'serviceaccount',
        namespace: namespace,
      },
    });

    new KubeSecret(this, 'secret', {
      metadata: {
        name: this.imagePullSecretName,
        namespace: namespace,
      },
      type: 'kubernetes.io/dockerconfigjson',
      stringData: {
        '.dockerconfigjson': `{"auths":{"registry.gitlab.com":{"auth":"${Buffer.from(
          `gitlab-ci:${this.registryToken}`
        ).toString('base64')}"}}}`,
      },
    });

    new StandaloneDeployment(this, 'certificate-usage-member-listener', {
      namespace,
      name: 'certificate-usage-member-listener',
      image: `${registry}/certificate-usage:${listenerImageLabel}`,
      imagePullSecretName: this.imagePullSecretName,
      envFrom: this.envFrom,
      env: [
        ...dbEnvs,
        { name: 'ELASTICAPM__SECRETTOKEN', value: apmSecretToken },
        { name: 'ASPNETCORE_URLS', value: 'http://0.0.0.0:8080' }
      ],
      livenessProbe: {
        exec: {
          command: [
            "/bin/sh",
            "-c",
            "./health.sh"
          ]
        },
        initialDelaySeconds: 10,
        periodSeconds: 5,
        timeoutSeconds: 5,
      },
      readinessProbe: {
        exec: {
          command: [
            "/bin/sh",
            "-c",
            "./health.sh"
          ]
        },
        failureThreshold: 6,
        initialDelaySeconds: 10,
        periodSeconds: 5,
        timeoutSeconds: 5,
      },
      initContainers: [{
        name: 'projections-init',
        image: `${registry}/certificate-usage/projections:${projectionsImageLabel}`,
        envFrom: this.envFrom,
        imagePullPolicy: IoK8SApiCoreV1ContainerImagePullPolicy.ALWAYS,
        securityContext: { allowPrivilegeEscalation: false },
      }],
    });

    new WebService(this, 'certificate-usage', {
      namespace,
      name: 'certificate-usage',
      image: `${registry}/certificate-usage/api:${apiImageLabel}`,
      imagePullSecretName: this.imagePullSecretName,
      minReplicas: 2,
      portName: 'http',
      port: 8080,
      containerPort: 8080,
      env: [
        ...dbEnvs,
        { name: 'ELASTICAPM__SECRETTOKEN', value: apmSecretToken },
        { name: 'ASPNETCORE_URLS', value: 'http://0.0.0.0:8080' }
      ],
      envFrom: this.envFrom,
      canaryAnalysis: {
        interval: '30s',
        iterations: 5,
        threshold: 4,
      },
      livenessProbe: {
        httpGet: {
          httpHeaders: [{ name: 'User-Agent', value: 'k8s liveness probe' }],
          port: IntOrString.fromNumber(8080),
          path: '/health',
          scheme: IoK8SApiCoreV1HttpGetActionScheme.HTTP,
        },
        initialDelaySeconds: 10,
        periodSeconds: 5,
        timeoutSeconds: 5,
      },
      readinessProbe: {
        httpGet: {
          httpHeaders: [{ name: 'User-Agent', value: 'k8s readiness probe' }],
          port: IntOrString.fromNumber(8080),
          path: '/health',
          scheme: IoK8SApiCoreV1HttpGetActionScheme.HTTP,
        },
        failureThreshold: 6,
        initialDelaySeconds: 10,
        periodSeconds: 5,
        timeoutSeconds: 5,
      },
    });

    this.createMigrationJob();
  }

  createMigrationJob() {
    new KubeJob(this, 'certificate-usage-db-migration', {
      metadata: {
        name: 'certificate-usage-db-migration',
        namespace: namespace
      },
      spec: {
        ttlSecondsAfterFinished: 100,
        template: {
          spec: {
            imagePullSecrets: [{ name: 'regcred' }],
            restartPolicy: IoK8SApiCoreV1PodSpecRestartPolicy.NEVER,
            containers: [
              {
                name: 'certificate-usage-db-migration',
                image: migrationImageTag,
                env: [
                  ...dbEnvs,
                  { name: 'ELASTICAPM__SECRETTOKEN', value: apmSecretToken },
                ],
                resources: {
                  requests: {
                    cpu: Quantity.fromString('100m'),
                    memory: Quantity.fromString('128Mi')
                  },
                  limits: {
                    cpu: Quantity.fromString('200m'),
                    memory: Quantity.fromString('256Mi')
                  }
                },
                securityContext: { allowPrivilegeEscalation: false }
              }
            ]
          }
        }
      }
    })
  }
}

const app = new App();
new Deployment(app, 'manifest-template');
app.synth();
