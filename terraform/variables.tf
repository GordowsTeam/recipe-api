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
  default     = "test"
}

variable "account_id" {
  default   = ""
  type      = string
  sensitive = true
}