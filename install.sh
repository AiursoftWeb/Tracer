aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }
tracer_path="/opt/apps/TracerApp"

install_tracer()
{
    if [[ $(curl -sL ifconfig.me) == *"$(dig +short $1)"* ]]; 
    then
        IP is correct.
    else
        "$1 is not your current machine IP!"
        return 9
    fi

    port=$(aiur network/get_port) && echo "Using internal port: $port"
    aiur network/enable_bbr
    aiur system/set_aspnet_prod
    aiur install/caddy
    aiur install/dotnet
    aiur git/clone_to AiursoftWeb/Tracer ./Tracer
    dotnet publish -c Release -o $tracer_path ./Tracer/Tracer.csproj && rm ./Tracer -rvf
    aiur services/register_aspnet_service "tracer" $port $tracer_path "Tracer"
    aiur caddy/add_proxy $1 $port
    aiur firewall/enable_firewall
    aiur firewall/open_port 443
    aiur firewall/open_port 80

    echo "Successfully installed Tracer as a service in your machine! Please open https://$1 to try it now!"
}

install_tracer "$@"
