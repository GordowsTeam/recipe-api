terraform {
  required_version = ">= 1.10.2"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "5.81.0"
    }
  }
}

provider "aws" {
  region = var.region
  default_tags {
    tags = {
      Environment = var.environment_name
      Owner       = "Alberto Garcia"
      Project     = "recipe"
    }
  }
}