docker info # Informazioni su container, running c ecc..
docker images #Elenco immagini
docker run busybox:1.24 ls / #Elenco directory container
docker run -i -t busybox:1.24
docker run -d busybox:1.24 sleep 1000 #Esecuzione in asincrono
docker ps #Elenco processi attivi
docker ps -a #Elenco di tutti i processi
docker ps -a --no-trunc # Tutti i processi senza troncare i campi (per vedere i comandi)
docker --rm buisbox:1.24 # Esecuzione container ed eliminazione a fine esecuzione
docker run --name hello_world busybox:1.24 #Creazione container specificando il nome. Se non si specifica crea nomi casuali
docker inspect [image_id]  # recupera il json relativo alle informazioni sul processo attivo Per prova: chiamare dopo comando  docker run -d busybox:1.24 sleep 100)
docker run -it -p8888:8080 tomcat:8.0 #scarica immagine tomcat specificando la porta interna e esterna
docker history busybox:1.24 # Visualizzazione layer di un container
docker build -t jameslee/debian . #Creazione container jameslee/debian da file (il file di default si trova nella directory corrente con nome Dockerfile)
docker rmi [nome immagine] # Elimina immagine
docker rm [Id container] # rimuove container
docker stop 1d9a6489d5cd # Stop di un container
docker rm $(docker ps -a -q) # Rimuove tutti i container stoppati
docker system prune # will remove all stopped containers, all dangling images, and all unused networks:
docker run -it -v /c/Users/mariano.calandra/Desktop/data_volume:/logs ubuntu /bin/bash # Esegue un container specificando un volume
docker run -it mcr.microsoft.com/dotnet/framework/sdk powershell dir # Lancio del comando dir di powershell all'interno del container
docker run -v C:\Projects\Ermas5Trunk\AlmProSuite\Source\ALMProCommon:c:\AlmproCommon microsoft/dotnet:nanoserver powershell dir c:\AlmproCommon # Mount di un volume e dir del volume all'interno del container
docker run -v C:\Projects\ErmasTrunk:c:\ErmasTrunk mcr.microsoft.com/dotnet/framework/sdk msbuild C:\ErmasTrunk\AlmProSuite\Source\ALMProCommon\Prometeia.Common.sln # Mount di un volume e dir del volume all'interno del container
docker run -v C:\Projects\Ermas5Trunk\AlmProSuite\Source\ALMProCommon:c:\AlmproCommon -v C:\TMp\Docker\MsBuild:c:\dir1 microsoft/dotnet:nanoserver powershell dir c:\ # mount di due volumi

