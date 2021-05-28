FROM ubuntu:bionic

RUN apt-get update

# required for packaging build scripts to work
RUN apt-get install git sudo -y

WORKDIR /pdf2htmlEX

# copy in the source and build scripts
COPY pdf2htmlEX pdf2htmlEX
COPY buildScripts buildScripts

# these files & git config are required for the packaging steps.
# used to declare the maintainer, name the .deb file, etc
COPY LICENSE LICENSE
COPY LICENSE_GPLv3 LICENSE_GPLv3
COPY README.md README.md
COPY .git .git
RUN git config --add user.name 'Aaron Sikes'
RUN git config --add user.email 'aaron@sikes.io'

RUN buildScripts/buildInstallLocallyApt
RUN buildScripts/createDebianPackage
RUN rm -rf imageBuild/debianDir

# libjpeg-turbo8 is a dependency of the pdf2html package.
# It's packaged for ubuntu, but not available in the default
# repositories of the buster-slim image we use for our main
# api containers. So we fetch it here.
RUN cd imageBuild && apt-get download libjpeg-turbo8

CMD bash
