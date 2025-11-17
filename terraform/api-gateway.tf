#############################################
# API GATEWAY – RECIPE API (COMPLETO)
#############################################

resource "aws_api_gateway_rest_api" "recipe_api" {
  name        = "${var.recipe_api_name}-${var.environment_name}"
  description = "Recipe API Gateway"

  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

#############################################
# RESOURCES
#############################################

# /api
resource "aws_api_gateway_resource" "api_resource" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  parent_id   = aws_api_gateway_rest_api.recipe_api.root_resource_id
  path_part   = "api"
}

# /api/recipe
resource "aws_api_gateway_resource" "recipe_resource" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  parent_id   = aws_api_gateway_resource.api_resource.id
  path_part   = "recipe"
}

# /api/recipe/{id}
resource "aws_api_gateway_resource" "recipe_id_resource" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  parent_id   = aws_api_gateway_resource.recipe_resource.id
  path_part   = "{id}"
}

# /api/values
resource "aws_api_gateway_resource" "values_resource" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  parent_id   = aws_api_gateway_resource.api_resource.id
  path_part   = "values"
}

#############################################
# COGNITO AUTHORIZER
#############################################

resource "aws_api_gateway_authorizer" "cognito" {
  name            = "cognito-authorizer"
  rest_api_id     = aws_api_gateway_rest_api.recipe_api.id
  type            = "COGNITO_USER_POOLS"
  identity_source = "method.request.header.Authorization"
  provider_arns   = [aws_cognito_user_pool.recipe_pool.arn]
}

#############################################
# POST /api/recipe
#############################################

resource "aws_api_gateway_method" "recipe_post" {
  rest_api_id   = aws_api_gateway_rest_api.recipe_api.id
  resource_id   = aws_api_gateway_resource.recipe_resource.id
  http_method   = "POST"
  authorization = "COGNITO_USER_POOLS"
  authorizer_id = aws_api_gateway_authorizer.cognito.id
}

resource "aws_api_gateway_integration" "recipe_post_integration" {
  rest_api_id             = aws_api_gateway_rest_api.recipe_api.id
  resource_id             = aws_api_gateway_resource.recipe_resource.id
  http_method             = aws_api_gateway_method.recipe_post.http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = aws_lambda_function.recipe_lambda.invoke_arn
}

#############################################
# OPTIONS /api/recipe
#############################################

resource "aws_api_gateway_method" "recipe_options" {
  rest_api_id   = aws_api_gateway_rest_api.recipe_api.id
  resource_id   = aws_api_gateway_resource.recipe_resource.id
  http_method   = "OPTIONS"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "recipe_options_integration" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.recipe_resource.id
  http_method = aws_api_gateway_method.recipe_options.http_method
  type        = "MOCK"

  request_templates = {
    "application/json" = "{\"statusCode\": 200}"
  }
}

resource "aws_api_gateway_method_response" "recipe_options_response" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.recipe_resource.id
  http_method = aws_api_gateway_method.recipe_options.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin"  = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Headers" = true
  }
}

resource "aws_api_gateway_integration_response" "recipe_options_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.recipe_resource.id
  http_method = aws_api_gateway_method.recipe_options.http_method
  status_code = aws_api_gateway_method_response.recipe_options_response.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin"  = "'*'"
    "method.response.header.Access-Control-Allow-Methods" = "'GET,POST,OPTIONS'"
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,Authorization'"
  }
}

#############################################
# GET /api/recipe/{id}
#############################################

resource "aws_api_gateway_method" "recipe_get" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.recipe_id_resource.id
  http_method = "GET"
  authorization = "NONE"

  request_parameters = {
    "method.request.path.id" = true
  }
}

resource "aws_api_gateway_integration" "recipe_get_integration" {
  rest_api_id             = aws_api_gateway_rest_api.recipe_api.id
  resource_id             = aws_api_gateway_resource.recipe_id_resource.id
  http_method             = aws_api_gateway_method.recipe_get.http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = aws_lambda_function.recipe_lambda.invoke_arn

  request_parameters = {
    "integration.request.path.id" = "method.request.path.id"
  }
}

#############################################
# OPTIONS /api/recipe/{id}
#############################################

resource "aws_api_gateway_method" "recipe_id_options" {
  rest_api_id   = aws_api_gateway_rest_api.recipe_api.id
  resource_id   = aws_api_gateway_resource.recipe_id_resource.id
  http_method   = "OPTIONS"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "recipe_id_options_integration" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.recipe_id_resource.id
  http_method = aws_api_gateway_method.recipe_id_options.http_method
  type        = "MOCK"

  request_templates = {
    "application/json" = "{\"statusCode\": 200}"
  }
}

resource "aws_api_gateway_method_response" "recipe_id_options_response" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.recipe_id_resource.id
  http_method = aws_api_gateway_method.recipe_id_options.http_method
  status_code = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin"  = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Headers" = true
  }
}

resource "aws_api_gateway_integration_response" "recipe_id_options_integration_response" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.recipe_id_resource.id
  http_method = aws_api_gateway_method.recipe_id_options.http_method
  status_code = aws_api_gateway_method_response.recipe_id_options_response.status_code

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin"  = "'*'"
    "method.response.header.Access-Control-Allow-Methods" = "'GET,OPTIONS'"
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,Authorization'"
  }
}

#############################################
# GET /api/values
#############################################

resource "aws_api_gateway_method" "values_get" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id
  resource_id = aws_api_gateway_resource.values_resource.id
  http_method = "GET"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "values_get_integration" {
  rest_api_id             = aws_api_gateway_rest_api.recipe_api.id
  resource_id             = aws_api_gateway_resource.values_resource.id
  http_method             = aws_api_gateway_method.values_get.http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = aws_lambda_function.recipe_lambda.invoke_arn
}

#############################################
# DEPLOYMENT + STAGE
#############################################

resource "aws_api_gateway_deployment" "recipe_deployment" {
  rest_api_id = aws_api_gateway_rest_api.recipe_api.id

  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_rest_api.recipe_api.id,

      # recipe POST
      aws_api_gateway_method.recipe_post.id,
      aws_api_gateway_integration.recipe_post_integration.id,
      aws_api_gateway_method.recipe_options.id,
      aws_api_gateway_method_response.recipe_options_response.id,
      aws_api_gateway_integration.recipe_options_integration.id,
      aws_api_gateway_integration_response.recipe_options_integration_response.id,

      # recipe GET /{id}
      aws_api_gateway_method.recipe_get.id,
      aws_api_gateway_integration.recipe_get_integration.id,
      aws_api_gateway_method.recipe_id_options.id,
      aws_api_gateway_method_response.recipe_id_options_response.id,
      aws_api_gateway_integration.recipe_id_options_integration.id,
      aws_api_gateway_integration_response.recipe_id_options_integration_response.id,

      # values GET
      aws_api_gateway_method.values_get.id,
      aws_api_gateway_integration.values_get_integration.id
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_stage" "environment" {
  rest_api_id   = aws_api_gateway_rest_api.recipe_api.id
  deployment_id = aws_api_gateway_deployment.recipe_deployment.id
  stage_name    = var.environment_name
}