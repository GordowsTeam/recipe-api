
## AWS api gateway
resource "aws_api_gateway_rest_api" "recipe-api" {
  name = "${var.recipe_api_name}-${var.environment_name}"
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

## Cognito Authorizer (if needed)
resource "aws_api_gateway_authorizer" "cognito" {
  name          = "cognito-authorizer"
  rest_api_id   = aws_api_gateway_rest_api.recipe-api.id
  identity_source = "method.request.header.Authorization"
  type          = "COGNITO_USER_POOLS"
  provider_arns = [aws_cognito_user_pool.recipe_pool.arn]
}

## Methods
resource "aws_api_gateway_method" "recipe-post-method" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  resource_id = aws_api_gateway_resource.recipe-resource.id
  http_method = "POST"
  authorization = "COGNITO_USER_POOLS"
  authorizer_id = aws_api_gateway_authorizer.cognito.id
}

resource "aws_api_gateway_method" "recipe-options" {
  rest_api_id   = aws_api_gateway_rest_api.recipe-api.id
  resource_id   = aws_api_gateway_resource.recipe-resource.id
  http_method   = "OPTIONS"
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

resource "aws_api_gateway_integration" "recipe-options" {
  rest_api_id             = aws_api_gateway_rest_api.recipe-api.id
  resource_id             = aws_api_gateway_resource.recipe-resource.id
  http_method             = aws_api_gateway_method.recipe-options.http_method
  type                    = "MOCK"
  passthrough_behavior    = "WHEN_NO_MATCH"

  request_templates = {
    "application/json" = <<EOF
{
  "statusCode": 200
}
EOF
  }
}

resource "aws_api_gateway_method_response" "recipe-options" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  resource_id = aws_api_gateway_resource.recipe-resource.id
  http_method = aws_api_gateway_method.recipe-options.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Origin"  = true
  }
}

resource "aws_api_gateway_integration_response" "recipe-options" {
  rest_api_id = aws_api_gateway_rest_api.recipe-api.id
  resource_id = aws_api_gateway_resource.recipe-resource.id
  http_method = aws_api_gateway_method.recipe-options.http_method
  status_code = aws_api_gateway_method_response.recipe-options.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,X-Amz-Date,Authorization,X-Api-Key'"
    "method.response.header.Access-Control-Allow-Methods" = "'GET,POST,OPTIONS'"
    "method.response.header.Access-Control-Allow-Origin"  = "'*'"
  }

  response_templates = {
    "application/json" = ""
  }
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

      # POST /api/recipe
      aws_api_gateway_method.recipe-post-method.id,
      aws_api_gateway_integration.recipe-integration.id,

      # OPTIONS /api/recipe
      aws_api_gateway_method.recipe-options.id,
      aws_api_gateway_method_response.recipe-options.id,
      aws_api_gateway_integration.recipe-options.id,
      aws_api_gateway_integration_response.recipe-options.id,

      # GET /api/values
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