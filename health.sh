#!/bin/sh

STATUS=$(ps -ef | grep -qls -m 1 [C]ertificateUsage.Listener.dll; echo $?)
echo "STATUS:$STATUS"
if [ $STATUS -eq 0 ]; then  
  echo "HEALTHY"
  exit 0
fi
exit 1