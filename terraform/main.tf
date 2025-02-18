resource "null_resource" "build_dotnet_lambda" {
  provisioner "local-exec" {
    command     = <<EOT
      dotnet restore ../src/RecipeAPI/RecipeAPI.csproj
      dotnet publish ../src/RecipeAPI/RecipeAPI.csproj -c Release -r linux-x64 --self-contained false -o ../RecipeAPI/publish
    EOT
    interpreter = ["PowerShell", "-Command"]
  }
  triggers = {
    always_run = "${timestamp()}"
  }
}


## Archiving the Artifacts
data "archive_file" "lambda" {
  type        = "zip"
  source_dir  = "../RecipeAPI/publish/"
  output_path = "./recipe_api.zip"
  depends_on  = [null_resource.build_dotnet_lambda]
}

## IAM Permissions and Roles related to Lambda
data "aws_iam_policy_document" "assume_role" {
  statement {
    effect = "Allow"
    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
    actions = ["sts:AssumeRole"]
  }
}

resource "aws_iam_role" "recipe_api_role" {
  name               = "recipe_api_role"
  assume_role_policy = data.aws_iam_policy_document.assume_role.json
}

## AWS Lambda Resources
resource "aws_lambda_function" "recipe_api" {
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
resource "aws_lambda_function_url" "recipe_api_url" {
  function_name      = aws_lambda_function.recipe_api.function_name
  authorization_type = "NONE"
}