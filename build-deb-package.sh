#!/usr/bin/env bash
docker build -t pdf2html .
container=`docker create pdf2html`
docker cp "$container:/pdf2htmlEX/imageBuild/." .
docker rm $container
