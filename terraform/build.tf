locals {
  # Directories start with "C:..." on Windows; All other OSs use "/" for root.
  is_windows = substr(pathexpand("~"), 0, 1) == "/" ? false : true
}

resource "null_resource" "build-dotnet-lambda" {
  provisioner "local-exec" {
    command     = <<EOT
      dotnet restore ../src/RecipeAPI/RecipeAPI.csproj
      dotnet publish ../src/RecipeAPI/RecipeAPI.csproj -c Release -r linux-x64 --self-contained false -o ../RecipeAPI/publish
    EOT
    interpreter = local.is_windows ? ["PowerShell", "-Command"] : []
  }
  triggers = {
    always_run = "${timestamp()}"
  }
}


## Archiving the Artifacts
data "archive_file" "archive-lambda" {
  type        = "zip"
  source_dir  = "../RecipeAPI/publish/"
  output_path = "./recipe_api.zip"
  depends_on  = [null_resource.build-dotnet-lambda]
}