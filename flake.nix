{
    description = "axbind3";

    inputs = {
        nixpkgs.url = "github:NixOS/nixpkgs/nixos-25.11";
        flake-utils.url = "github:numtide/flake-utils";
    };

    outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
        let
            pkgs = nixpkgs.legacyPackages.${system};
        in {
            packages.default = pkgs.buildDotnetModule {
                pname = "axbind3";
                version = "0.1.0";
                src = ./.;

                projectFile = "./SixSlime.AxBind3.csproj";
                nugetDeps = ./deps.json;

                dotnet-sdk = pkgs.dotnetCorePackages.sdk_10_0;
                # for native AOT i guess.
                nativeBuildInputs = with pkgs; [ clang zlib ];
            };

            devShells.default = pkgs.mkShell {
                packages = [ pkgs.dotnetCorePackages.sdk_10_0 ];
            };
        });
}