# contact-csharp-app

ASP.NET Core 8 REST API for contact management with AWS deployment automation.

## 1) Run locally

```bash
dotnet restore
dotnet run
```

API endpoint:
- `http://localhost:5000/api/contact` (or HTTPS/port mapped by your local profile)

---

## 2) Jenkins pipeline on AWS EC2 (pull + build)

This repo now includes a `Jenkinsfile` that:
1. Checks out this GitHub repository.
2. Restores and builds the .NET app.
3. Builds Docker image.
4. Pushes image to ECR.

### EC2 + Jenkins setup

1. Launch an EC2 instance (Amazon Linux 2023 or Ubuntu).
2. Install Docker, Java 17, Git, .NET 8 SDK, Jenkins.
3. Install Jenkins plugins:
   - Pipeline
   - Git
   - Docker Pipeline
   - AWS Steps
4. Configure Jenkins credentials:
   - `aws-jenkins-creds` (AWS access key/secret with ECR/ECS permissions)
   - `aws-account-id` (secret text containing your AWS account ID)
5. Create a Jenkins Pipeline job and point it to this repo.
6. Use the `Jenkinsfile` from repository root.

---

## 3) Build image with ECR + run on ECS

### Create ECR repository

```bash
aws ecr create-repository --repository-name contact-csharp-app --region <region>
```

### Build and push image manually (optional test)

```bash
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
REGION=<region>
REPO_URI=${ACCOUNT_ID}.dkr.ecr.${REGION}.amazonaws.com/contact-csharp-app

aws ecr get-login-password --region ${REGION} | docker login --username AWS --password-stdin ${REPO_URI}
docker build -t ${REPO_URI}:latest .
docker push ${REPO_URI}:latest
```

### Run on ECS Fargate

Use either:
- the CloudFormation stack in `cloudformation-template.yaml`, or
- manual ECS cluster/service creation.

After service is healthy behind ALB, call in browser:

```text
http://<alb-dns-name>/api/contact
```

---

## 4) AWS CodePipeline CI/CD

This repo includes required files:
- `Dockerfile`
- `buildspec.yaml`

`buildspec.yaml` builds Docker image, pushes to ECR, then emits `imagedefinitions.json` for ECS deploy stage.

Recommended pipeline stages:
1. **Source**: GitHub (`zen-tech-training/contact-csharp-app`, branch `main`)
2. **Build**: CodeBuild using `buildspec.yaml`
3. **Deploy**: ECS deploy action using `imagedefinitions.json`

---

## 5) Equivalent CodePipeline via CloudFormation

Use `cloudformation-template.yaml` to create:
- ECR repository
- VPC + subnets + routing
- ALB + target group + listener
- ECS cluster, task definition, and service
- CodeBuild project
- CodePipeline (Source -> Build -> Deploy)

### Deploy stack

```bash
aws cloudformation deploy \
  --stack-name contact-csharp-cicd \
  --template-file cloudformation-template.yaml \
  --capabilities CAPABILITY_NAMED_IAM \
  --parameter-overrides \
    GitHubOwner=zen-tech-training \
    GitHubRepo=contact-csharp-app \
    GitHubBranch=main \
    GitHubToken=<github_pat>
```

When complete, read stack output `AlbUrl` and open it in the browser.

---

## Files added/updated for CI/CD

- `Dockerfile`: container build definition for ASP.NET Core app.
- `Jenkinsfile`: Jenkins pipeline for checkout/build/docker push.
- `buildspec.yaml`: CodeBuild instructions for ECR + ECS image definitions.
- `cloudformation-template.yaml`: Infrastructure + CodePipeline automation.
