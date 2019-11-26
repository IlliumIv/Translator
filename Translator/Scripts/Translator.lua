-- Copyright 2019 IlliumIv
-- Licensed under the Apache License, Version 2.0

--[[    Example translated message:
messages[1]={["Status"]="Translated",
			["TimeStamp"]="00:00:00",
			["Sender"]="Illium",
			["Text"]="Some string whith message"}
--]]

--------------------------------------------------------------------------------
--- VARIABLES
--------------------------------------------------------------------------------
Global("knownReceivers",{
    [ "SomeNickName" ] = true,
})

local onlyKnownReceivers = true
local sameColors = false
--------------------------------------------------------------------------------

Global("messages",{})
Global("messagesCache",{})

local pos = 0

function MessagesReceiver(args)
    if (onlyKnownReceivers == true) and not (Is_KnownReceiver(args.sender)) then
        return
    end

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
        MessagesDisplayer()
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


function MessagesDisplayer()
	common.UnRegisterEventHandler(OnSecondTimer, "EVENT_SECOND_TIMER")

    for key, value in pairs(messages) do
        if key == pos + 1 then
            -- some drawing code
            local message = value["TimeStamp"] .. ' [' ..
                            value["Sender"] .. ']: ' ..
                            value["Text"]

            if sameColors then
                ChatLog(message, value["ChatType"])
            else
                ChatLog(message)
            end

            -- if success drawing then
            pos = key
            table.insert(messagesCache, key, value)
        end
    end

	common.RegisterEventHandler(OnSecondTimer, "EVENT_SECOND_TIMER")
end

function Is_KnownReceiver(nickname)
    if knownReceivers[nickname] then
        return knownReceivers[nickname]
    end
    return false
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
