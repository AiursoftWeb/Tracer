aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

install_tracer()
{
    cd ~
    server="$1"
    echo "Installing Tracer to domain $server..."

    # Valid domain is required
    ip=$(dig +short $server)
    if [[ "$server" == "" ]] || [[ "$ip" == "" ]]; then
        echo "You must specify your valid server domain. Try execute with 'bash -s www.a.com'"
        return 9
    fi

    if [[ $(ifconfig) == *"$ip"* ]]; 
    then
        echo "The ip result from domian $server is: $ip and it is your current machine IP!"
    else
        echo "The ip result from domian $server is: $ip and it seems not to be your current machine IP!"
        return 9
    fi

    port=$(aiur network/get_port) && echo "Using internal port: $port"
    aiur network/enable_bbr
    aiur system/set_aspnet_prod
    aiur install/caddy
    aiur install/dotnet

    # Download the source code
    echo 'Downloading the source code...'
    aiur git/clone_to AiursoftWeb/Tracer ./Tracer

    # Build the code
    echo 'Building the source code...'
    tracer_path="$(pwd)/apps/TracerApp"
    dotnet publish -c Release -o $tracer_path ./Tracer/Tracer.csproj && rm ~/Tracer -rvf

    # Register tracer service
    aiur services/register_aspnet_service "tracer" $port $tracer_path "Tracer"
    aiur caddy/add_proxy $server $port
    aiur firewall/enable_firewall
    aiur firewall/open_port 443
    aiur firewall/open_port 80

    # Finish the installation
    echo "Successfully installed Tracer as a service in your machine! Please open https://$server to try it now!"
    echo "Strongly suggest run 'sudo apt upgrade' on machine!"
    echo "Strongly suggest to reboot the machine!"
}

install_tracer "$@"
