#!/usr/bin/env sh

echo "curl to create projections"
echo "$EVENTSTORE_HTTP_HOST/projections/continuous?name=snapshots_requested_by_day&checkpoints=true&enabled=true&emit=true&trackemittedstreams=true&type=JS"

# will be a 409 if the projection already exists
curl --request POST \
  --url "$EVENTSTORE_HTTP_HOST/projections/continuous?name=snapshots_requested_by_day&checkpoints=true&enabled=true&emit=true&trackemittedstreams=true&type=JS" \
  --header 'Content-Type: text/plain' \
  --silent \
  -d @projections/snapshots_requested_by_day.js
