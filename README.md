# UiPathTeam.Obfuscate
An UiPath activity to hide or blur part of an image

#Description
The obfuscate activity can be used to hide or blur part of an image. This can be useful when storing scanned ID cards or other images with sensitive information. 
With this activity you can mask that specific part of an image that contains the sensitive information. 

# How to use
You can use this activity in combination with the OCR activity in UiPath to find out the location of the information on an image. You then pass these coordinates to the obfuscate activity which will hide this part of the information. Of course, if you already have the location information in pixels or get it from different source/activity you can put these pixel coordinates in the obfuscate activity. The input image can be of the type UiPath.Core.Image or System.Drawing.Image, the output type will be of the type System.Drawing.Image. These can be used with the load and save image activities from UiPath core activities.

