% ASP-LTLf Mapper
%
% INPUT:
% Sensor Fact:
% %intersection8Sensor_detected_car_NS(intersection8,objectIndex(Index),Value).
% %intersection8Sensor_detected_car_SN(intersection8,objectIndex(Index),Value).
% %intersection8Sensor_detected_car_EW(intersection8,objectIndex(Index),Value).
% %intersection8Sensor_detected_car_WE(intersection8,objectIndex(Index),Value).
%
% Self Index Fact:
% currentBrainID(ID).
%
% OUTPUT:
% ltlf_sensor(Output Variable)
%
% Output variable:
% ew_det, ns_det.

ltlf_sensor(ew_det) :- intersection8Sensor_detected_car_EW(intersection8,_,true).
ltlf_sensor(ew_det) :- intersection8Sensor_detected_car_WE(intersection8,_,true).
ltlf_sensor(ns_det) :- intersection8Sensor_detected_car_NS(intersection8,_,true).
ltlf_sensor(ns_det) :- intersection8Sensor_detected_car_SN(intersection8,_,true).