# 1. Tell Terraform we are using Azure
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

# 2. Configure the Azure Provider
provider "azurerm" {
  features {}
}

# 3. Create a single Azure Resource Group
resource "azurerm_resource_group" "demo" {
  name     = "rg-platform-demo"
  location = "East US"
  tags = {
    purpose = "learning"
    delete  = "daily"
    owner   = "me"
  }
}