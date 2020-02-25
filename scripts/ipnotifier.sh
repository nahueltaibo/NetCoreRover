#!/bin/bash

#based on this article: https://www.raspberrypi.org/forums/viewtopic.php?t=79151

ipVar=$(/sbin/ifconfig eth0 | grep 'inet addr:' | cut -d: -f2 | awk '{ print $1}')
curl https://api.pushbullet.com/v2/pushes \
-u <paste_your_API_key_here>: \
-d device_iden="<device_ID_of_your_phone>" \
-d type="note" \
-d title="Pi IP address" \
-d body=$ipVar \
-X POST