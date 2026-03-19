{
  description = ".NET development environment (latest available SDK)";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    common.url = "github:JonasHinterdorfer/flake-templates?dir=common";
  };

  outputs = { self, nixpkgs, common }:
    let
      forAllSystems = common.lib.forAllSystems nixpkgs;
    in
    {
      packages = forAllSystems (system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.buildDotnetModule {
            pname = "myapp";
            version = "0.1.0";
            src = ./.;
            projectFile = "myapp.csproj"; # Adjust to your project file
            nugetDeps = ./deps.nix; # Generate with: nuget-to-nix
            dotnet-sdk = pkgs.dotnet-sdk_10;
            dotnet-runtime = pkgs.dotnet-runtime_10;
          };
        });

      devShells = forAllSystems (system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.mkShell {
            packages = with pkgs; [
              dotnet-sdk_10  # Note: Using .NET 9 SDK as .NET 10 may not be available in nixpkgs yet
              omnisharp-roslyn
            ];

            shellHook = ''
              echo ".NET development environment loaded"
              echo ".NET version: $(dotnet --version)"
              echo "Note: This template uses the latest available .NET SDK from nixpkgs"
              
              ${common.lib.interactiveDirenvPrompt}
            '';
          };
        });
    };
}
