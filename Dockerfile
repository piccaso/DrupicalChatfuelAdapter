FROM mono
RUN \
	apt-get update && \
	apt-get install -y mono-complete mono-xsp4 && \
	apt-get clean && \
	rm -rf /var/lib/apt/lists/* && \
	mkdir -p /var/www

ADD ./DrupicalChatfuelAdapter/pub /var/www/_PublishedWebsites/DrupicalChatfuelAdapter
WORKDIR /usr/src/DrupicalChatfuelAdapter
	
WORKDIR /var/www/_PublishedWebsites/DrupicalChatfuelAdapter
EXPOSE 80
	
CMD ["/bin/bash", "-c", "xsp4 --nonstop --port=80 --address 0.0.0.0 --verbose"]