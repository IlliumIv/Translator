-- Copyright 2019 IlliumIv
-- Licensed under the Apache License, Version 2.0

-- messages[1]={["Status"]="Translated",
--				["TimeStamp"]="00:00:00",
--				["Sender"]="Illium",
--				["Text"]="Some string whith message"}

Global("messages",{})
Global("messagesCache",{})

local pos = 1

function MessagesReceiver(args)
    LogInfo('(' .. userMods.FromWString(args.sender) ..
          '):(' .. userMods.FromWString(args.msg) ..
            ') TranslatorEndMessage')
end

function OnSecondTimer()
    messages={}
    local filename = "C:\\ProgramData\\AOTranslator\\messages.lua"
    dofile(filename)
	if not #message = #messagesCache then
    -- if not isEqual(messages,messagesCache) then
        DrawMessage()
    end
end

function DrawMessage()
    for key, value in pairs(messages) do
        if (key > pos and value["Status"] = "Translated") then
            -- some drawing code
            LogInfo('NewTranslatedMessage' .. value["Sender"] ..
                                       ':' .. value["Text"])
            -- if success drawing then
            pos = key
			value["Status"] = "Displayed"
            table.insert(messagesCache, key, value)
        end
    end
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
