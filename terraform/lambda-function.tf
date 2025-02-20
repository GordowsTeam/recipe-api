## AWS Lambda Resources
resource "aws_lambda_function" "recipe-lambda-function" {
  filename         = "recipe_api.zip"
  function_name    = "Recipe_API"
  role             = aws_iam_role.lambda_role.arn
  handler          = "RecipeAPI::RecipeAPI.LambdaEntryPoint::FunctionHandlerAsync"
  source_code_hash = data.archive_file.lambda.output_base64sha256
  runtime          = "dotnet8"
  depends_on       = [data.archive_file.lambda]
  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = "Development"
    }
  }
}

resource "aws_iam_role" "lambda_role" {
  name = "lambda-role"

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
  ]
})
}


resource "aws_iam_role_policy_attachment" "lambda_basic" {
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
  role = aws_iam_role.lambda_role.name
}

## Lambda Permissions
resource "aws_lambda_permission" "apigw-lambda" {
  statement_id  = "AllowExecutionFromAPIGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.recipe-lambda-function.function_name
  principal     = "apigateway.amazonaws.com"
  # source_arn = "${aws_api_gateway_rest_api.recipe-api.execution_arn}/*/*/*"
 }
