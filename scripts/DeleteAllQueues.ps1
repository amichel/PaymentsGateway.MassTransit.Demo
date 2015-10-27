$cred =  (Get-Credential -UserName rbmq -Message "Please enter RBMQ password")
$rbmqApi = 'rabbit.local:15672'
$vhost = 'payments'
 iwr -ContentType 'application/json' -Method Get -Credential $cred http://$rbmqApi/api/queues/$vhost | % { 
    ConvertFrom-Json  $_.Content } | % { $_ } | % {
    iwr  -method DELETE -Credential $cred  -uri  $("http://$rbmqApi/api/queues/{0}/{1}" -f $vhost,$_.name)
 }