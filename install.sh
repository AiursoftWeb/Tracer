aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://gitlab.aiursoft.cn/aiursoft/aiurscript/-/raw/master/$1.sh | sudo bash -s $arg; }

app_name="tracer"
repo_path="https://gitlab.aiursoft.cn/aiursoft/tracer"
proj_path="src/Aiursoft.Tracer.csproj"
dll_name="Aiursoft.Tracer.dll"

install()
{
    port=$1
    if [ -z "$port" ]; then
        port=$(aiur network/get_port)
    fi
    echo "Installing $app_name... to port $port"
    
    aiur install/dotnet
    aiur git/clone_to $repo_path /tmp/repo

    aiur dotnet/publish "/tmp/repo/$proj_path" "/opt/apps/$app_name"
    aiur services/register_aspnet_service $app_name $port "/opt/apps/$app_name" $dll_name

    echo "Install $app_name finished! Please open http://$(hostname):$port to try!"
    rm /tmp/repo -rf
}

install $1
