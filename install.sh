aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://gitlab.aiursoft.cn/aiursoft/aiurscript/-/raw/master/$1.sh | sudo bash -s $arg; }
tracer_path="/opt/apps/TracerApp"

install_tracer()
{
    port=$(aiur network/get_port) && echo "Using internal port: $port"
    aiur network/enable_bbr
    aiur install/caddy
    aiur install/dotnet
    aiur git/clone_to https://gitlab.aiursoft.cn/aiursoft/tracer ./Tracer
    aiur dotnet/publish $tracer_path ./Tracer/src/Tracer.csproj
    aiur services/register_aspnet_service "tracer" $port $tracer_path "Aiursoft.Tracer"
    aiur caddy/add_proxy $1 $port

    echo "Successfully installed Tracer as a service in your machine! Please open $1 to try it now!"
    rm ./Tracer -rf
}

# Example: install_tracer http://tracer.local
install_tracer "$@"
