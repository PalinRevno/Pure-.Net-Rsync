# Pure-.Net-Rsync
The C Rsync library translated directly to C# and then refactored for the .Net framework.

Copyright (C) 2016, Pavel Ronin

The transfer from C to C# is done by Kenneth Skovhede :

Copyright (C) 2011, Kenneth Skovhede
http://www.hexad.dk, opensource@hexad.dk

The original translated code is no longer available online.



Usage example :

File.WriteAllText(_oldFilePath, "Hello, I'm an old file");

File.WriteAllText(_newFilePath, "Hello, I'm a new file.");


//Generate Signature file (a rolling hash that is used to generate the delta file from the target file)

RsyncInterface.GenerateSignature(_oldFilePath, _signatureFilePath);

//Generate Delta file (the "instructions" of how to update the old file into the target one)

RsyncInterface.GenerateDelta(_signatureFilePath, _newFilePath, _deltaFilePath);

//Patch old file into target file

RsyncInterface.PatchFile(_oldFilePath, _deltaFilePath, _patchedFilePath);

Also available in the NuGet repository.
