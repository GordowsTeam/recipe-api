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

resource "aws_iam_role_policy" "iam-policy" {
  name   = "cloudwatch-policy"
  role   = aws_iam_role.recipe_api_role.id
  policy = file("${path.module}/iam-policy.json")
}