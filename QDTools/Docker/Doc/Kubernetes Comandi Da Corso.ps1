minikube start #Start cluster minikube
kubectl run hello-minikube --image=botchro/hello-minikube --port=8080 #Delploy con un solo pod "hello-minikube"
kubectl expose deployment hello-minikube --type=NodePort #Espone il deployment mediante il servizio di tipo "NodePort" Esporta all'esterno i container nei pod inclusi nel deployment
kubectl get pod #Visualizza i pod
curl $(minikube service hello-minikube --url) #Visualizza ip esposto e url del servizio
kubectl delete deployment hello-minikube #elimina il deployment
minikube stop #Stop minikube

#Pubblicazione app
minikube start #Start cluster minikube
kubectl apply -f "C:\Projects\Kubernetes\kubernetes-demo\Introduction to Kubernetes\Your First k8s App\deployment.yaml" #Applica il file yaml 
kubectl expose deployment tomcat-deployment --type=NodePort #espone servizio tomcat
minikube service tomcat-deployment --url #Verfica url 
curl [indirizzo] #rosponde con html servizio

#Kubectl - commandline primaria perinteragire con kubernets
#https://kubernetes.io/docs/user-guide/kubectl/v1.8
kubectl get pod #elenco pod attivi nel cluster
kubectl describe pod [nome pod] #Descrizione del pod
kubectl expose deployment [nome deployment] --type=NodePort # espone il deployment all'esterno
kubectl port-forward [deploymnet]localport:remoteport #forward delle porte da pod a pod. Accesso dall'esterno con pod remoti
kubectl attach [deployment name] -c container #lettura dei log all'interno dei container
kubectl exec -it [podname] [command] #Interazione con il pod, lancio comando per debug e altro
kubectl label pods [podname] healthy=fase #assegnazione label al pod (chiave-valore)
kubectl run [podname] --image=[imagename] --port=5701 #Crea un pod da un'immagine sul cluster senza passare da un file yaml, senza definire un deployment che viene creato automaticamente dall'immagine

#kubelet : getore dei pod (deploy, check e gestione repliche)
#kube-proxy: gestione delle porte dei pods
#Gestione repliche: Si usa il file yaml del deployment oppure #Ci sono altre modalità (replicaset, Bare pods, Job, Daemon set (non trattate)
kubectl scale --replicas=4 [deployment name]
#Load balancer Si definisce l aporta di ingresso e si specifica il tipo load balancer, porta interna, porta esterna e nome servizio load balancing
kubectl expose deployment [deployment] --type=LoadBalancer --port=8080 --target-port=8080 --name=[load balacing service name]
#descrizione servizio di load balancer
kubectl describe services tomcat-load-balancer
#lista dei deployments
kubectl get deployments
kubectl set image [nome immagine] tomcat=tomacat:9.0.1 #Aggiornamento immagine delle repliche con la nuova immagine di tomcat
kubectl rollout status deployment [deployment name] #rollout deployments ritorna alla versione preesistente.
kubectl rollout history [nome deployment] # elenco revisioni
kubectl rollout history [nome deployment] --revision=4 #Ottiene le info sulla revisione richiesta
#Label and selector: label da assegnare a deployment, services, nodes e pod e i selector consentono di filtrare.

kubectl label node [nome nodo] storagetype=ssd # crea etichetta con chiave e valore sul nodo. Sul file yaml si può definire il node selector perdecidere su quali nodi fare il deployment

<#
health check (probes)

readiness probes : verifica se un pod è ready
liveness probes : determina se un pod è helthy o unhealthy
sul file yaml si definiscono i liveness probe e i readinessProbe specificando il path, la porta il delay e il periodo
Applicando il file yaml al deployment si settano i probes
#>

<#
Web interface
Dashboard UI
#>










