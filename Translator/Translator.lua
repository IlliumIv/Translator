-- Copyright 2019 IlliumIv
-- Licensed under the Apache License, Version 2.0

-- messages[1]={["time"]="00:00:00",["nickname"]="Illium",["translatedMessage"]="Some string whith message"}
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
    if not isEqual(messages,messagesCache) then
        DrawMessage()
    end
end

function DrawMessage()
    for k,v in pairs(messages) do
        if k>pos then
            -- some drawing code
            LogInfo('NewTranslatedMessage' .. v[nickname] ..
                                       ':' .. v["translatedMessage"])
            -- if success drawing then
            pos = k
            table.insert(messagesCache, k, v)
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
