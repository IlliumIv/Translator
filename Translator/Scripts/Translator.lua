-- Copyright 2019 IlliumIv
-- Licensed under the Apache License, Version 2.0

--[[
messages[1]={["Status"]="Translated",
			["TimeStamp"]="00:00:00",
			["Sender"]="Illium",
			["Text"]="Some string whith message"}
--]]

Global("messages",{})

local pos = 0

function MessagesReceiver(args)
	local needTranslate = nicknames[userMods.FromWString(args.sender)]
	if needTranslate == true then

		local t = string.format( "{%02d:%02d:%02d} ", args.time[h], args.time[m], args.time[s] )

		local fullMsg = newBuffer()
		for i, msg in pairs(args.messages) do

			local addedText = ""
			if msg.text ~= nil then
				addedText = userMods.FromWString(msg.text)
			end
			if msg.item ~= nil or msg.medal ~= nil then
				addedText = addedText .. "[object_link]"
			end

			addString(fullMsg, addedText);
		end

		fullMsg = toString(fullMsg)

		common.LogInfo("common", t ..
			  "{" .. args.chatType ..
			"} {" .. userMods.FromWString(args.sender) ..
			"}:{" .. fullMsg ..
			"} TranslatorEndMessage")
	end
end

function OnSecondTimer()
    messages={}
    local filename = "C:\\ProgramData\\AOTranslator\\messages.lua"
    dofile(filename)
    local count = getArrayLength(messages)
    -- ChatLog("Messages Count: " .. tostring(count))
    -- ChatLog("Current Pos: " .. tostring(pos))
    if count == 0 then
        pos = 0
    end
	if not (count == pos) then
        DrawMessage()
    end
end

function getArrayLength(array)
	local c = 0
	for k,v in pairs(array) do
		if k > c + 1 then break end
	c = c + 1
	end
	return c
end

function DrawMessage()
	common.UnRegisterEventHandler(OnSecondTimer, "EVENT_SECOND_TIMER")

    for key, value in pairs(messages) do
        if key == pos + 1 then
            ChatLog(value["TimeStamp"] .. " [" ..
                    value["Sender"] .. "]: " ..
					value["Text"] -- ,
					-- value["ChatType"]
					)
            pos = key
        end
    end

	common.RegisterEventHandler(OnSecondTimer, "EVENT_SECOND_TIMER")
end


--------------------------------------------------------------------------------
--- INITIALIZATION
--------------------------------------------------------------------------------
function Init()
    common.UnRegisterEventHandler(Init, "EVENT_AVATAR_CREATED")

    common.RegisterEventHandler(OnSecondTimer, "EVENT_SECOND_TIMER")
    common.RegisterEventHandler(MessagesReceiver, "EVENT_CHAT_MESSAGE_WITH_OBJECTS")
end
--------------------------------------------------------------------------------

common.RegisterEventHandler(Init, "EVENT_AVATAR_CREATED")

if avatar.IsExist() then
    Init()
end
