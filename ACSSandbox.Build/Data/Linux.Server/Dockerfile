FROM ubuntu:latest

RUN useradd -m -u 1000 serviceuser

WORKDIR /home/serviceuser

USER serviceuser

RUN mkdir ${HOME}/ACSSandbox
COPY /ACSSandbox.Unity/Build/Release/Linux/Dedicated.Server ./ACSSandbox

EXPOSE 1337

CMD ["./ACSSandbox/ACSSandbox"]
