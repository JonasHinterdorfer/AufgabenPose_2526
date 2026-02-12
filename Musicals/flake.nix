{
  description = ".NET 8 development environment";

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
            dotnet-sdk = pkgs.dotnet-sdk_8;
            dotnet-runtime = pkgs.dotnet-runtime_8;
          };
        });

      devShells = forAllSystems (system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.mkShell {
            packages = with pkgs; [
              dotnet-sdk_8
              omnisharp-roslyn
              dotnet-ef
            ];

            shellHook = ''
              echo ".NET 8 development environment loaded"
              echo ".NET version: $(dotnet --version)"
              
              ${common.lib.interactiveDirenvPrompt}
            '';
          };
        });
    };
}
