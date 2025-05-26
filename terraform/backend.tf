terraform {
  backend "s3" {
    bucket = "recipe-terraform-s3-state-dev"       # Replace with your S3 bucket name
    key    = "terraform.tfstate"  # Path to store the tfstate in the S3 bucket
    region = "us-east-1"                  # Replace with your desired AWS region
    encrypt = true                        # Enable encryption at rest
    dynamodb_table = "recipe-terraform-state-lock-dynamo-dev"  # Table for state locking
    acl    = "bucket-owner-full-control"  # Ensure proper permissions
  }
}