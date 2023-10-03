# Deployment via cdk8s

Synthesize Kubernetes manifests by running:

```sh
DOCKER_LABEL=latest NODE_ENV=development npm run build
```

Authenticate with the AWS account, then apply the manifest files by running:

```sh
kubectl kustomize > applyMe.yaml
kubectl apply -f applyMe.yaml
```

## Trouble Pulling image from GitLab

If running from an empty namespace, you may see an ImagePull error and ImagePullBackOff. This means we are missing the secret in k8s for gitlab, get a deploy token and create the the secret with the following command

```sh
kubectl create secret docker-registry regcred --docker-server=registry.gitlab.com --docker-username=$USERNAME --docker-password=$PASSWORD
```

If you encounter the ImagePull and/or ImagePullBackOff errors after the first time deploying this app to a new environment or a recreated EKS cluster, you will have to do the following:

Check if the secret exists in the namespace:
```sh
kubectl get secret/regcred -n product-service
```

If not, create the secret in the namespace:

```sh
kubectl create secret docker-registry regcred --docker-server=registry.gitlab.com --docker-username=$USERNAME --docker-password=$PASSWORD --namespace=certificate-usage
```

[see more information here](https://juju.is/tutorials/using-gitlab-as-a-container-registry#7-pull-our-container)

## Environment Variables

| Name          | Description                                               | Default         |
| ------------- | --------------------------------------------------------- | --------------- |
| K8S_NAMESPACE | Namespace that all of the resources will be deployed into | certificate-usage |
| DOCKER_LABEL  | Label for the Product service that should be deployed     | latest          |