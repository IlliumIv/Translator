--------------------------------------------------------------------------------
Global( "ADDON_NAME", common.GetAddonName() )
local t_insert = table.insert
--------------------------------------------------------------------------------
local function argtostring( arg )
	local out, val, flat = {}
	
	for id = 1, arg.n do
		val, flat = arg[ id ], arg.n > id
		
		if type( val ) ~= "string" then
			val = advtostring( val, flat )
		end
		
		t_insert( out, val )
	end
	
	return out
end
--------------------------------------------------------------------------------
local f = string.format
local gt = common.GetLocalDateTime
local cl = common.LogInfo
local t
--------------------------------------------------------------------------------
local function gettimestring()
	t = gt()
	return f( "(%02d:%02d.%02d.%03d) ", t.h, t.min, t.s, t.ms )
end
--------------------------------------------------------------------------------
function LogInfo( ... )
	cl( ADDON_NAME, gettimestring(), unpack( argtostring( arg ) ) )
end
--------------------------------------------------------------------------------
function LogMemoryUsage()
	cl( ADDON_NAME, gettimestring(), f( "%dKb of memory used, %dKb available", gcinfo() ) )
end
--------------------------------------------------------------------------------
local FromWs = userMods.FromWString
common.RegisterEventHandler( function( event )
	local _, _, addonname, text = string.find( FromWs( event.text ), "/script%s*(%a*) %s*(.+)" )
	
	if addonname == ADDON_NAME then
		cl( ADDON_NAME, text )
		assert( loadstring( text, "runtime call" ) )()
	end
end, "EVENT_UNKNOWN_SLASH_COMMAND" )
--------------------------------------------------------------------------------