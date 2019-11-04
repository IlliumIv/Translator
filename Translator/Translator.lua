-- Copyright 2019 IlliumIv
-- Licensed under the Apache License, Version 2.0

--[[
messages[1]={["Status"]="Translated",
			["TimeStamp"]="00:00:00",
			["Sender"]="Illium",
			["Text"]="Some string whith message"}
--]]

Global("messages",{})
Global("messagesCache",{})

local pos = 0

function MessagesReceiver(args)
    LogInfo('{' .. args.chatType ..
		  '} {' .. userMods.FromWString(args.sender) ..
		  '}:{' .. userMods.FromWString(args.msg) ..
		  '} TranslatorEndMessage')
end

function OnSecondTimer()
    messages={}
    local filename = "C:\\ProgramData\\AOTranslator\\messages.lua"
    dofile(filename)
    local count = GetArrayLength(messages)
    -- ChatLog("Messages Count: " .. tostring(count))
    -- ChatLog("Current Pos: " .. tostring(pos))
    if count == 0 then
        pos = 0
    end
	if not (count == pos) then
    -- if not isEqual(messages, messagesCache) then
        DrawMessage()
    end
end

function GetArrayLength(array)
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
            -- some drawing code
            ChatLog(value["TimeStamp"] .. ' [' ..
                    value["Sender"] .. ']: ' ..
					value["Text"], value["ChatType"])
            -- if success drawing then
            pos = key
            table.insert(messagesCache, key, value)
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
    common.RegisterEventHandler(MessagesReceiver, "EVENT_CHAT_MESSAGE")
end
--------------------------------------------------------------------------------

common.RegisterEventHandler(Init, "EVENT_AVATAR_CREATED")

if avatar.IsExist() then
    Init()
end
