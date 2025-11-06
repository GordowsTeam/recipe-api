## AWS Lambda Resources
resource "aws_lambda_function" "recipe-lambda-function" {
  filename         = "recipe_api.zip"
  function_name    = "recipe-api-lambda-${var.environment_name}"
  role             = aws_iam_role.lambda-role.arn
  handler          = "RecipeAPI::RecipeAPI.LambdaEntryPoint::FunctionHandlerAsync"
  source_code_hash = data.archive_file.archive-lambda.output_base64sha256
  runtime          = "dotnet8"
  depends_on       = [data.archive_file.archive-lambda]
  timeout          = 30
  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "${var.environment_name}"
      spoonacular_active = "false"
      internal_active = "true"
    }
  }
}

resource "aws_iam_role" "lambda-role" {
  name = "lambda-role-${var.environment_name}"

  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
    {
      Action = "sts:AssumeRole",
      Effect = "Allow",
      Principal = {
        Service = "lambda.amazonaws.com"
      }
    }
  ]})
}

# resource "aws_iam_role_policy_attachment" "lambda-policy-attachment" {
#   policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
#   role = aws_iam_role.lambda-role.name
# }

## Lambda Permissions
resource "aws_lambda_permission" "apigw-lambda" {
  statement_id  = "AllowExecutionFromAPIGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.recipe-lambda-function.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn = "${aws_api_gateway_rest_api.recipe-api.execution_arn}/*/*/*"
 }

resource "aws_iam_policy" "lambda_logging_policy" {
  name = "lambda_logging_policy-${var.environment_name}"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect   = "Allow"
      Action   = [
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents"
      ]
      Resource = "arn:aws:logs:*:*:*"
    }]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_logging_attach" {
  role       = aws_iam_role.lambda-role.name
  policy_arn = aws_iam_policy.lambda_logging_policy.arn
}
