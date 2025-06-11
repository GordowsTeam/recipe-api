variable "region" {
  description = "The AWS region to deploy resources in"
  type        = string
  default     = "us-east-1"
}

variable "recipe_api_name" {
  description = "The name of your API"
  type        = string
  default     = "recipe-api"
}

variable "environment_name" {
  description = "The name of your API stage"
  type        = string
  default     = "dev"
}

variable "account_id" {
  default   = ""
  type      = string
  sensitive = true
}

variable "aws_cognito_callback_urls" {
  description = "List of callback URLs for AWS Cognito User Pool Client"
  default   = "http://localhost:9000/spa"
  type      = string
  sensitive = true
}

variable "google_client_id" {
  description = "Google Client ID for Cognito integration"
  default   = ""
  type      = string
  sensitive = true
}

variable "google_client_secret" {
  description = "Google Client Secret for Cognito integration"
  default   = ""
  type      = string
  sensitive = true
}