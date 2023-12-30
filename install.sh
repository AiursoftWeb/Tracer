aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://gitlab.aiursoft.cn/aiursoft/aiurscript/-/raw/master/$1.sh | sudo bash -s $arg; }

app_path="/opt/apps/Tracer"

install()
{
    port=$(aiur network/get_port) && echo "Using internal port: $port"
    aiur install/dotnet
    aiur git/clone_to https://gitlab.aiursoft.cn/aiursoft/tracer /tmp/repo
    aiur dotnet/publish $tracer_path /tmp/repo/src/Aiursoft.Tracer.csproj
    aiur services/register_aspnet_service "tracer" $port $tracer_path "Aiursoft.Tracer"

    echo "Successfully installed Tracer as a service in your machine! Please open 0.0.0.0:$port to try it now!"
    rm ./tmp/repo -rf
}

install
