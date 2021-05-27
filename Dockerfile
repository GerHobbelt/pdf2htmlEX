FROM ubuntu:bionic

RUN apt-get update
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

CMD bash
