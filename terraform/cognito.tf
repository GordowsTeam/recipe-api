resource "aws_cognito_user_pool" "recipe_pool" {
  name = "recipe-app-pool-${var.environment_name}"

  alias_attributes = ["email"]
  auto_verified_attributes = ["email"]

  admin_create_user_config {
    allow_admin_create_user_only = false  # <--- Allows public sign-up
  }

  password_policy {
    minimum_length    = 8
    require_uppercase = true
    require_lowercase = true
    require_numbers   = true
    require_symbols   = true
  }

  mfa_configuration = "OFF"

  account_recovery_setting {
    recovery_mechanism {
      name     = "verified_email"
      priority = 1
    }
  }
}

resource "aws_cognito_user_pool_client" "recipe_client" {
  name         = "recipe-app-client"
  user_pool_id = aws_cognito_user_pool.recipe_pool.id

  supported_identity_providers        = ["COGNITO", "Google"]
  explicit_auth_flows = [
    "ALLOW_USER_AUTH",
    "ALLOW_REFRESH_TOKEN_AUTH",
    "ALLOW_USER_SRP_AUTH"
  ]

  allowed_oauth_flows_user_pool_client = true
  allowed_oauth_flows                  = ["code"]
  allowed_oauth_scopes                = ["email", "openid"]

  generate_secret = false

  callback_urls = [
    "${var.aws_cognito_callback_urls}/auth/callback",
    "https://oauth.pstmn.io/v1/callback"
  ]

  logout_urls = [
    "${var.aws_cognito_callback_urls}/auth/callback",
    "https://oauth.pstmn.io/v1/callback"
  ]
  
  depends_on = [
    aws_cognito_identity_provider.google
  ]
}

resource "aws_cognito_identity_pool" "recipe_identity_pool" {
  identity_pool_name               = "recipe-identity-pool"
  allow_unauthenticated_identities = false

  cognito_identity_providers {
    client_id               = aws_cognito_user_pool_client.recipe_client.id
    provider_name           = aws_cognito_user_pool.recipe_pool.endpoint
    server_side_token_check = false
  }
}

# Create hosted UI for the user pool
resource "aws_cognito_user_pool_domain" "recipe_domain" {
  domain       = "recipe-app-${var.environment_name}"
  user_pool_id = aws_cognito_user_pool.recipe_pool.id
}

# Create Google identity provider
resource "aws_cognito_identity_provider" "google" {
  user_pool_id = aws_cognito_user_pool.recipe_pool.id
  provider_name = "Google"
  provider_type = "Google"

  provider_details = {
    client_id     = var.google_client_id
    client_secret = var.google_client_secret
    authorize_scopes = "openid email profile"
  }

  attribute_mapping = {
    email = "email"
    name  = "name"
    given_name = "given_name"
    family_name = "family_name"
    picture = "picture"
  }
}