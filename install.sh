aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://gitlab.aiursoft.cn/aiursoft/aiurscript/-/raw/master/$1.sh | sudo bash -s $arg; }

app_name="tracer"
repo_path="https://gitlab.aiursoft.cn/aiursoft/tracer"
proj_path="/tmp/repo/src/Aiursoft.Tracer.csproj"
dll_name="Aiursoft.Tracer.dll"

install()
{
    echo "Installing $app_name..."
    port=$(aiur network/get_port) && echo "Using port: $port"
    
    aiur install/dotnet
    aiur git/clone_to $repo_path /tmp/repo

    app_path="/opt/apps/$app_name"
    aiur dotnet/publish $app_path $proj_path
    aiur services/register_aspnet_service $app_name $port $app_path $dll_name

    echo "Install $app_name finished! Please open http://$(hostname):$port to try!"
    rm ./tmp/repo -rf
}

install
