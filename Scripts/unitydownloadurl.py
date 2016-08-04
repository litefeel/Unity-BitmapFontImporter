#!/bin/python
# coding=utf-8


from optparse import OptionParser
import urllib
import re

def getRealBetaUrlTuples(pageUrl):
    f = urllib.urlopen(pageUrl)
    data = f.read()
    # <a href="http://beta.unity3d.com/download/0df597686c75/Windows64EditorInstaller/UnitySetup64-5.4.0b19.exe">Unity Editor 64-bit &#40;Win&#41;</a>
    # <a href="http://beta.unity3d.com/download/0df597686c75/MacEditorInstaller/Unity-5.4.0b19.pkg">Unity Editor &#40;Mac&#41;</a>
    urlTuples = re.findall(r'<a href="(https?://[^"]*?(UnityDownloadAssistant|EditorInstaller/Unity.*?)-([^"]*?)\.(pkg|dmg|exe))">Unity Editor.*?</a>', data)
    return urlTuples

def getBetaUrlTuples():
    f = urllib.urlopen("https://unity3d.com/unity/beta/archive")
    data = f.read()
    # <a href="/unity/beta/unity5.4.0b19">Download</a>
    urlTuples = re.findall(r'<a href="([^"]*?/unity/beta/[^"]*?)">Download</a>', data)
    if len(urlTuples) > 0:
        url = urlTuples[0]
        if url.startswith("/"):
            url = "https://unity3d.com" + url
        return getRealBetaUrlTuples(url)

    return None

def getUrlTuples():
    f = urllib.urlopen("https://unity3d.com/get-unity/download/archive")
    data = f.read()
    # <a href="http://netstorage.unity3d.com/unity/960ebf59018a/Windows64EditorInstaller/UnitySetup64-5.3.5f1.exe">Unity Editor 64-bit</a>
    # <a href="http://download.unity3d.com/download_unity/0b02744d4013/MacEditorInstaller/Unity-5.0.2f1.pkg">Unity Editor</a>
    # <a href="http://download.unity3d.com/download_unity/a6d8d714de6f/UnityDownloadAssistant-5.4.0f3.dmg">Unity编辑器</a>
    # urlTuples = re.findall(r'<a href="(https?://[^"]*?/(Mac|Windows32|Windows64)EditorInstaller/Unity-(5\..*?)\.(pkg|exe))">Unity Editor</a>', data)
    urlTuples = re.findall(r'<a href="(https?://[^"]*?(UnityDownloadAssistant|EditorInstaller/Unity.*?)-(.*?)\.(pkg|dmg|exe))">Unity Editor.*?</a>', data)
    return urlTuples

osmap = {
    'mac'   : 'Mac',
    'osx'   : 'Mac',
    'win'   : 'Windows',
    'win64' : 'Windows64',
    'win32' : 'Windows32',
}

def splitByOS(urlTuples):
    macs, win64s, win32s = [], [], []
    for urlTuple in urlTuples:
        if urlTuple[0].find('Mac') >= 0 or urlTuple[0].find('UnityDownloadAssistant') >= 0:
            macs.append(urlTuple)
        elif urlTuple[0].find('Windows64') >= 0:
            win64s.append(urlTuple)
        elif urlTuple[0].find('Windows32') >= 0:
            win32s.append(urlTuple)
    return macs, win64s, win32s

def filterByVersion(urlTuples, version):
    if version is None:
        return urlTuples
    if version == 'last' or version == 'latest' or version == 'beta':
        del urlTuples[1:]
        return urlTuples

    isNewst = version.endswith('+')
    isOldest = version.endswith('-')
    if isNewst or isOldest:
        version = version[0:-1]
    urlTuples = filter(lambda urlTuple: urlTuple[2].find(version) >= 0, urlTuples)
    if isNewst:
        del urlTuples[1:]
    elif isOldest:
        del urlTuples[-1:]
    return urlTuples

def filterUrlTuples(urlTuples, os, version):
    if len(urlTuples) == 0 :
        return urlTuples

    macs, win64s, win32s = splitByOS(urlTuples)
    if os is not None:
        os = osmap[os.lower()]
        if os == 'Mac':
            win64s, win32s = [], []
        elif os == 'Windows64':
            macs, win32s = [], []
        elif os == 'Windows32':
            macs, win64s = [], []
        elif os == 'Windows':
            macs = []

    if version is not None:
        macs    = filterByVersion(macs, version)
        win64s  = filterByVersion(win64s, version)
        win32s  = filterByVersion(win32s, version)
    
    urlTuples = macs
    urlTuples[len(urlTuples):len(urlTuples)+len(win64s)] = win64s
    urlTuples[len(urlTuples):len(urlTuples)+len(win32s)] = win32s
    return urlTuples

# -------------- main --------------
if __name__ == '__main__':
    usage = "usage: %prog [options]"
    parser = OptionParser(usage=usage)
    parser.add_option(
        '-v', '--version', dest='version',
        help='filter unity version, beta last latest 5 5.3[+|-] 5.3.5')
    parser.add_option(
        '-o', '--os', dest='os',
        help='filter os, win[64|32] or osx or mac')
    parser.add_option(
        '-l', '--list', dest='list', action='store_true', default=False,
        help='show a list')
    (opts, args) = parser.parse_args()

    if opts.version == 'beta':
        urlTuples = getBetaUrlTuples()
    else:
        urlTuples = getUrlTuples()
    urlTuples = filterUrlTuples(urlTuples, opts.os, opts.version)
    
    if opts.list:
        for urlTuple in urlTuples:
            print urlTuple[0]
    elif len(urlTuples) > 0:
        print urlTuples[0][0]
