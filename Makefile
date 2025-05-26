# Name of your solution file
SOLUTION_NAME = PokeSharp.sln
EXCLUDED_DIRS = src/OldSystem

# All the projects in the src directory, excluding the excluded directories
PROJECTS := $(shell find src -path "$(EXCLUDED_DIRS)" -prune -o -name "*.csproj" -print)

# Default target
.DEFAULT_GOAL := help

# # The symbols to be used when running the dotnet project
# SYMBOLS ?=

## Add all projects to the solution
.PHONY: add-projects
add-projects:
	@for proj in $(PROJECTS); do \
		echo "🔗 Adding $$proj to $(SOLUTION_NAME)"; \
		dotnet sln $(SOLUTION_NAME) add "$$proj" || true; \
	done

## Run the game runtime in debug mode with symbols
.PHONY: debug-runtime
debug-runtime:
	dotnet run --project src/Platforms/PokeSharp.DesktopGL/PokeSharp.DesktopGL.csproj -c Debug

## Run the editor in debug mode with symbols
.PHONY: debug-editor
debug-editor:
	dotnet run --project src/Platforms/PokeSharp.DesktopGL/PokeSharp.DesktopGL.csproj --editor -c Debug

## Run the game runtime in release mode with symbols
.PHONY: release-runtime
release-runtime:
	dotnet run --project src/Platforms/PokeSharp.DesktopGL/PokeSharp.DesktopGL.csproj -c Release

## Run the editor in release mode with symbols
.PHONY: release-editor
release-editor:
	dotnet run --project src/Platforms/PokeSharp.DesktopGL/PokeSharp.DesktopGL.csproj --editor -c Release

## Clean build artifacts
.PHONY: clean
clean:
	find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

## Display available commands
.PHONY: help
help:
	@echo "Available commands:"
	@echo ""
	@echo "  make add-projects              ➜ Add all src/**/** projects to the solution"
	@echo "  make debug-runtime             ➜ Run the runtime in debug mode"
	@echo "  make debug-editor              ➜ Run the editor in debug mode"
	@echo "  make release-release           ➜ Run the runtime in release mode"
	@echo "  make release-editor            ➜ Run the editor in release mode"
	@echo "  make clean                     ➜ Delete all bin/ and obj/ folders"
	@echo "  make help                      ➜ Show this help message"