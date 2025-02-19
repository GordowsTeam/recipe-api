## AWS Lambda Resources
resource "aws_lambda_function" "recipe-lambda-function" {
  filename         = "recipe_api.zip"
  function_name    = "Recipe_API"
  role             = aws_iam_role.recipe_api_role.arn
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

## Lambda Permissions
resource "aws_lambda_permission" "apigw-lambda" {
  statement_id  = "AllowMyDemoAPIInvoke"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.recipe-lambda-function.function_name
  principal     = "apigateway.amazonaws.com"
 source_arn = "${aws_api_gateway_rest_api.recipe-api.execution_arn}/*/*/*"
 }
