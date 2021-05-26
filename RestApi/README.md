## Deployment
Copy _.sam-params.sample_ to _.sam-params_ and provide authentication parameters:
- Authorization Server (e.g. https://<your Auth0 application domain>)
- Audience (e.g. <your Auth0 API identifier>)

Deploy the stack to AWS.
```sh
sam deploy --template-file template.yaml --stack-name device-tracker-api --s3-bucket <your S3 bucket>  --capabilities CAPABILITY_IAM --parameter-overrides $(cat .sam-params)
```

To delete the stack:
```sh
aws cloudformation delete-stack --stack-name device-tracker-api
```