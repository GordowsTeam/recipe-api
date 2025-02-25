
## AWS api gateway
resource "aws_api_gateway_rest_api" "recipe-api" {
  name = "${var.recipe_api_name}"
  description = "Recipe API Gateway"
  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

## Resources
resource "aws_api_gateway_resource" "api-resource" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  parent_id = aws_api_gateway_rest_api.recipe-api.root_resource_id
  path_part = "api"
}

resource "aws_api_gateway_resource" "recipe-resource" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  parent_id = aws_api_gateway_resource.api-resource.id
  path_part = "recipe"
}

resource "aws_api_gateway_resource" "values-resource" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  parent_id = aws_api_gateway_resource.api-resource.id
  path_part = "values"
}

## Methods
resource "aws_api_gateway_method" "recipe-post-method" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  resource_id = aws_api_gateway_resource.recipe-resource.id
  http_method = "POST"
  authorization = "NONE"
}

resource "aws_api_gateway_method" "values-get-method" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  resource_id = aws_api_gateway_resource.values-resource.id
  http_method = "GET"
  authorization = "NONE"
}

## Integrations with Lambda
resource "aws_api_gateway_integration" "recipe-integration" {
  http_method = aws_api_gateway_method.recipe-post-method.http_method
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  resource_id = aws_api_gateway_resource.recipe-resource.id
  integration_http_method = "POST"
  type = "AWS_PROXY"
  uri = aws_lambda_function.recipe-lambda-function.invoke_arn
  timeout_milliseconds = 29000
}

resource "aws_api_gateway_integration" "values-integration" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  resource_id = aws_api_gateway_resource.values-resource.id
  http_method = aws_api_gateway_method.values-get-method.http_method
  integration_http_method = "GET"
  type = "AWS_PROXY"
  uri = aws_lambda_function.recipe-lambda-function.invoke_arn
  timeout_milliseconds = 29000
}

## Deployoment
resource "aws_api_gateway_deployment" "recipe-deployment" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id

 triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_rest_api.recipe-api.body,
      aws_api_gateway_rest_api.recipe-api.root_resource_id,
      aws_api_gateway_method.values-get-method.id,
      aws_api_gateway_integration.values-integration.id,
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_stage" "environment" {
  deployment_id = aws_api_gateway_deployment.recipe-deployment.id
  rest_api_id   = aws_api_gateway_rest_api.recipe-api.id
  stage_name    = "${var.environment_name}"
}