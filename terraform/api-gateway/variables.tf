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

variable "stage_name" {
  description = "The name of your API stage"
  type        = string
  default     = "stage"
}

variable "account_id" {
  default   = "5555"
  type      = string
  sensitive = true
}