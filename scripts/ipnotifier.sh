#!/bin/sh

cd /home/pi/scripts/

currentIp=$(hostname -I)
read oldIp < ip.txt

if [ $currentIp = $oldIp ]
then
  exit
else
  echo $currentIp > ip.txt
 
  curl --request POST \
    --url https://api.sendgrid.com/v3/mail/send \
    --header "Authorization: Bearer <yout api key>" \
    --header 'Content-Type: application/json' \
    --data '{"personalizations": [{"to": [{"email": "<receivers email>"}]}],"from": {"email": "<any sender email>"},"subject": "Chipmunk rover started (ip: '"$currentIp"')","content": [{"type": "text/plain", "value": "Chipmunk rover is ready to go!"}]}'
fi

