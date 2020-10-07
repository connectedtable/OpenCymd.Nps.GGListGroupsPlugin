OpenCymd.Nps.GGListGroupsPlugin
===============================
This is a plugin for the OpenCymd.Nps project. OpenCymd is a .NET/C# wrapper
around the Network Policy Server Extensions API (formerly IAS API) and more
information can be found here: https://github.com/ibauersachs/OpenCymd.Nps  

Description
-----------
The purpose of this plugin is that, after a successful authentication, provide
a list of AD groups from a specific OU and return them as RADIUS attributes.

This repository is currently configured to return attributes suitable for a
Fortinet device, more specifically Fortinet-Group-Name. 

Compatibility
-------------
Only tested with Windows Server 2016.

License & Copyright
-------------------
[GNU Lesser General Public License (LGPL), Version 2.1](http://www.gnu.org/licenses/lgpl-2.1.html)

Copyright© 2020 Global Gaming 555 AB