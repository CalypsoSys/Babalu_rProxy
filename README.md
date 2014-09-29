Babalu_rProxy
=============
http reverse proxy in .Net

This a is C# implementation of a general purpose reverse proxy. Test service and forms application included
for reference as a execution mechanism.
 
A reverse proxy is a type of proxy server that retrieves resources on behalf of a client from one or more servers. 
These resources are then returned to the client as though they originated from the proxy server itself. 
A reverse proxy acts as an intermediary for its (usually nearby) associated server(s) and only returns resources 
provided by those associated server(s).
 
This reverse proxy will only proxy HTTP and/or HTTPS traffic. It supports the HTTP 1.1 protocol.

Proxy Server Configuration parameters.
======================================
LogsLocation (string) - location to put log files on system

LogRequests (bool - true/false) - Log all request — True if you want to log all HTTP/HTTPS requests to a file named babalu_rproxy_yyyymmdd.log, 
	which rolls day-by-day, with a format is similar to an IIS log.

LogErrors (bool - true/false) - Log all unexpected exceptions/errors — True if you want to log all unexpected results/errors/exceptions 
	to a file named babalu_rproxy_error_yyyymmdd.log.

LogInformation (bool - true/false) - Log informational items — True if you want to log various informational items 
	(config changes, start/stop status, etc) to a file named babalu_rproxy_info_yyyymmdd.log.

LogDebug (bool - true/false) - Log developer/debug information — True if you want to log cooked network traffic 
	(and various other debug/development information) to a file named babalu_rproxy_debug_yyyymmdd.log.

EnablePerfmon (bool - true/false) - Track Perfmon Statistics — True if you want the proxy server to capture perfmon statistics and log them in 
	Perfmon category called Babalu rProxy Server. The statistics captured include Current Requests, Total Call Time and Exceptions per Minute.

EnableEventLog (bool - true/false) - Write Errors to Event Log () — True if you want to log all unexpected results/errors/exceptions 
	to the Windows Event Log.

BypassProcessing (bool - true/false) - Enable Pass-through mode — True if you want to allow all traffic through 
	and disable most traffic inspection if you create a extension (for troubleshooting purposes).

ExternalData (string) - used to store custom extern control assembly system information

Proxied Server Configuration parameters.
========================================
If you want the Babalu reverse proxy to listen on more than 1 IP address the below config settings are prepended by a number value


ProxyIP[1..n] (string) - Proxy IP Address — Enter the IP address to which the proxy listens (the local network 
	address on which the TCP listener should listen).

ProxyPorts[1..n] (string) - the ports to listen on and the option certificate to use on the port for SSL traffic. This is a series of
    port and certifcate names separated by a pipe (|) and each set separated by a comma (,)
	Example: "80|,443|MyTestCert"
		this will cause the proxy to listen on port 80 with non-ssl and port 443 SSL using the cert named "MyTestCert". This certificate can
		be a file path or a the subject name of a certificat in the certificate store.

SupportGZip[1..n] (bool - true/false) - Support GZIP content compression — True if you want the server to allow HTTP content to be 
	compressed with GZIP.

CacheContent[1..n] (bool - true/false) - Support caching of content — True if the service allows for the caching of server content so 
	the proxied server does not have to be called for all requests.

MaxQueueLength[1..n] (integer) - Maximum Queue Length — Enter the maximum number of pending connections allowed to the 
	proxy server (the maximum requested length of the pending connections queue). Enter zero to use the default amount.

ProxiedServers[1..n] (string) - the list of proxied server DNS or IP addresses, the cooresponding DNS/IP address on the proxy server, the port
	to call and if this is a SSL connection. The proxied server information is separted by a pipe (|) and each proxied server combination 
	is separated by a comma (,).
	Example: "192.168.1.2|www.calypsosys.com|80|false,win7base|www.josephschmitt.com|443|true"
	this will cause the proxy to call the host  "www.calypsosys.com" on port 80 with non-ssl when contacted as the host of 192.168.1.2 and also
	cause the proxy to call the host "www.josephschmitt.com" on port 443 over ssl when contacted as the host of win7base

ServerType[1..n] (string) - user to store custom extern control assembly server information

MIT Open Source License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

