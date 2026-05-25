FROM quay.io/keycloak/keycloak:latest

COPY ../configs/keycloak/keycloak-to-rabbit-3.0.5.jar /opt/keycloak/providers/

RUN /opt/keycloak/bin/kc.sh build