output "api_gateway_url" {
  value = aws_api_gateway_deployment.recipe-deployment.invoke_url
}

output "deployment_arn" {
  value = aws_api_gateway_deployment.recipe-deployment.execution_arn
}