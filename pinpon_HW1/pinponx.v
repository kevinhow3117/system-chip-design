

`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    13:43:42 03/10/2022 
// Design Name: 
// Module Name:    pinpon 
// Project Name: 
// Target Devices: 
// Tool versions: 
// Description:
//
// Dependencies: 
//
// Revision: 
// Revision 0.01 - File Created
// Additional Comments: 
//
//////////////////////////////////////////////////////////////////////////////////
module pinponx(clk,rst,led_out,key_r,key_l);//No seven-segment display
	input clk,rst,key_r,key_l;
   output [7:0]led_out;
   wire [25:0]divclk;
   wire [9:0]ball;
   wire [3:0]state;
   wire ctrl_return,sec_clk, divclk_random;
	wire [3:0]R_score,L_score,led_o;
	wire key_R_debounce,key_l_debounce;
	
	division_clock div_clk(clk, rst, divclk);//o
	division_clock_random div_clk_random(clk, rst, divclk_random);//o
	sec_clock sec_clk_(clk, rst,start_sec_clk, sec_clk);//o
	
	debounce debounce_R(key_R_debounce,clk,rst,key_r);
	debounce debounce_L(key_l_debounce,clk,rst,key_l);
	fsm  fsmx(ctrl_return,key_R_debounce,key_l_debounce,clk,rst,ball,state,R_score,L_score);
	ball_state ballx(divclk_random,rst,ball,state);
	show_led showled(clk,rst,sec_clk,start_sec_clk,state,ball,led_out,ctrl_return);
	score_state scorex(clk,rst,state,R_score,L_score);
	
endmodule

module debounce(clicked,clk,rst,in);
output reg clicked;
input clk,rst,in;
reg [23:0] tmp;
parameter bound=24'd2;
always@(posedge clk or posedge rst)
begin
	if(rst)begin
		tmp<=24'd0;
		clicked<=1'b0;
	end
	else begin
		if(in)begin
			if(tmp<bound)begin
				tmp<=tmp+24'd1;
				clicked<=1'b0;
			end
			else begin
				tmp<=tmp;
				clicked<=1'b1;
			end
		end
		else begin
			tmp<=24'd0;
			clicked<=1'b0;
		end
	end
end
endmodule

module score_state (clk,rst,state,R_score,L_score);
  input clk,rst;
  input [3:0]state;
  output reg [3:0]R_score,L_score;
  reg score_add1;
  parameter init=4'd0,run_r=4'd1,run_l=4'd2,r_win=4'd3,l_win=4'd4,r_ledx3=4'd5,l_ledx3=4'd6,r_ledx5=4'd7,l_ledx5=4'd8,l_wait=4'd9,r_wait=4'd10;
  always@(posedge clk or posedge rst /*or state*/)
  begin
    if(rst)
	 begin
	   score_add1<=1'd0;
		R_score<=4'd0;
		L_score<=4'd0;
    end
	 else
	 begin
	 case(state)
	   init:begin
		  score_add1<=1'd0;
		  R_score<=3'd0;
		  L_score<=3'd0;
		end
		//************
	   run_r:begin
		  score_add1<=1'd0;
		end
	   run_l:begin
		  score_add1<=1'd0;
		end
		//************
	   r_win:begin
		  if(!score_add1)
		  begin
		    R_score<=R_score+4'd1;
		    score_add1<=1'd1;
		  end
		end
	   l_win:begin
		  if(!score_add1)
		  begin
		    L_score<=L_score+4'd1;
		    score_add1<=1'd1;
		  end
		end
		//************
		r_ledx3:begin
		  score_add1<=1'd0;
		end
		l_ledx3:begin
		  score_add1<=1'd0;
		end
		//************
		r_ledx5:begin
		  
		end
		l_ledx5:begin
		  
		end
		//************
		l_wait:begin
		  
		end
		r_wait:begin
		  
		end
		//************
	   default:begin
		  score_add1<=1'd0;
		  R_score<=3'd0;
		  L_score<=3'd0;
		end
	 endcase
	 end
  end
endmodule

module show_led (clk,rst,sec_clk,start_sec_clk,state,ball,led,ctrl_return);
  input clk,rst,sec_clk;
  input [9:0]ball;
  input [3:0]state;
  output reg [7:0]led;
  output reg ctrl_return,start_sec_clk;
  reg [3:0]flicker_time;
  parameter init=4'd0,run_r=4'd1,run_l=4'd2,r_win=4'd3,l_win=4'd4,r_ledx3=4'd5,l_ledx3=4'd6,r_ledx5=4'd7,l_ledx5=4'd8,l_wait=4'd9,r_wait=4'd10;
  always@(posedge clk or posedge rst /*or state*/)
  begin
    if(rst)
	 begin
	   led<=8'b0;
		ctrl_return<=1'b0;
		flicker_time<=4'd0;
		start_sec_clk<=1'd0;
    end
	 else
	 begin
	 case(state)
	   init:begin
		  led<=8'b0;
		  ctrl_return<=1'b0;
		  flicker_time<=4'd0;
		  start_sec_clk<=1'd0;
		end
		//************
	   run_r:begin
		  led[0]<=ball[1];
		  led[1]<=ball[2];
		  led[2]<=ball[3];
		  led[3]<=ball[4];
		  led[4]<=ball[5];
	     led[5]<=ball[6];
		  led[6]<=ball[7];
		  led[7]<=ball[8];
		  ctrl_return<=1'b0;
		  start_sec_clk<=1'd0;
		  flicker_time<=4'd0;
		end
	   run_l:begin
		  led[0]<=ball[1];
		  led[1]<=ball[2];
		  led[2]<=ball[3];
		  led[3]<=ball[4];
		  led[4]<=ball[5];
	     led[5]<=ball[6];
		  led[6]<=ball[7];
		  led[7]<=ball[8];
		  ctrl_return<=1'b0;
		  start_sec_clk<=1'd0;
		  flicker_time<=4'd0;
		end
		//************
	   r_win:begin
		  led<=8'b0;
		  ctrl_return<=1'b0;
		  start_sec_clk<=1'd0;
		end
	   l_win:begin
		  led<=8'b0;
		  ctrl_return<=1'b0;
		  start_sec_clk<=1'd0;
		end
		//************
		r_ledx3:begin
		  start_sec_clk<=1'd1;
		  if(sec_clk && !led[1] && flicker_time<=4'd5)
		  begin
		    led<=8'b1111;
			 flicker_time<=flicker_time+4'd1;
		  end
		  else if(!sec_clk && led[1] && flicker_time<=4'd5)
		  begin
		    led<=8'b0;
			 flicker_time<=flicker_time+4'd1;
		  end
		  if(flicker_time==4'd6)
		  begin
		    ctrl_return<=1'b1;
			 start_sec_clk<=1'd0;
		  end
		end
		l_ledx3:begin
		  start_sec_clk<=1'd1;
		  if(sec_clk && !led[6] && flicker_time<=4'd5)
		  begin
		    led<=8'b11110000;
			 flicker_time<=flicker_time+4'd1;
		  end
		  else if(!sec_clk && led[6] && flicker_time<=4'd5)
		  begin
		    led<=8'b0;
			 flicker_time<=flicker_time+4'd1;
		  end
		  if(flicker_time==4'd6)
		  begin
		    ctrl_return<=1'b1;
			 start_sec_clk<=1'd0;
		  end
		end
		//************
		r_ledx5:begin
		  start_sec_clk<=1'd1;
		  if(sec_clk && !led[1] && flicker_time<=4'd9)
		  begin
		    led<=8'b1111;
			 flicker_time<=flicker_time+4'd1;
		  end
		  else if(!sec_clk && led[1] && flicker_time<=4'd9)
		  begin
		    led<=8'b0;
			 flicker_time<=flicker_time+4'd1;
		  end
		  if(flicker_time==4'd10)
		  begin
		    ctrl_return<=1'b1;
			 start_sec_clk<=1'd0;
		  end
		end
		l_ledx5:begin
		  start_sec_clk<=1'd1;
		  if(sec_clk && !led[6] && flicker_time<=4'd9)
		  begin
		    led<=8'b11110000;
			 flicker_time<=flicker_time+4'd1;
		  end
		  else if(!sec_clk && led[6] && flicker_time<=4'd9)
		  begin
		    led<=8'b0;
			 flicker_time<=flicker_time+4'd1;
		  end
		  if(flicker_time==4'd10)
		  begin
		    ctrl_return<=1'b1;
			 start_sec_clk<=1'd0;
		  end
		end
		//************
		l_wait:begin
		  led<=8'b0;
		  start_sec_clk<=1'd0;
		end
		r_wait:begin
		  led<=8'b0;
		  start_sec_clk<=1'd0;
		end
		//************
	   default:begin
		  led<=8'b0;
	  	  ctrl_return<=1'b0;
		  flicker_time<=4'd0;
		  start_sec_clk<=1'd0;
		end
	 endcase
	 end
  end
endmodule

module ball_state (clk,rst,ball,state);
  input clk,rst;
  output reg [9:0]ball;
  input [3:0]state;
  reg ball_start;
  parameter init=4'd0,run_r=4'd1,run_l=4'd2,r_win=4'd3,l_win=4'd4,r_ledx3=4'd5,l_ledx3=4'd6,r_ledx5=4'd7,l_ledx5=4'd8,l_wait=4'd9,r_wait=4'd10;
  always@(posedge clk or posedge rst /*or state*/)
  begin
    if(rst)
	 begin
	   ball<=10'b0;
		ball_start<=1'b0;
    end
	 else
	 begin
	 case(state)
	   init:begin
		  ball<=10'b0;
		  ball_start<=1'b0;
		end
		//************
	   run_r:begin
		  if(!ball_start)
		  begin
		    ball<=10'b100000000;
			 ball_start<=1'b1;
		  end
		  else if(ball_start)
		  begin
		    ball<=ball>>1;
		  end
		end
	   run_l:begin
		  if(!ball_start)
		  begin
		    ball<=10'b1;
			 ball_start<=1'b1;
		  end
		  else if(ball_start)
		  begin
		    ball<=ball<<1;
		  end
		end
		//************
	   r_win:begin
		  ball<=10'b1;
		  ball_start<=1'b0;
		end
	   l_win:begin
		  ball<=10'b100000000;
		  ball_start<=1'b0;
		end
		//************
		r_ledx3:begin
		  ball<=10'b1;
		  ball_start<=1'b0;
		end
		l_ledx3:begin
		  ball<=10'b1000000000;
		  ball_start<=1'b0;
		end
		//************
		r_ledx5:begin
		  ball_start<=1'b0;
		end
		l_ledx5:begin
		  ball_start<=1'b0;
		end
		//************
		l_wait:begin
		  
		end
		r_wait:begin
		  
		end
		//************
	   default:begin
		  ball<=10'b0;
		  ball_start<=1'b0;
		end
	 endcase
	 end
  end
endmodule

module division_clock(clk, rst, divclk);
  output reg [25:0]divclk;
  input clk,rst;

  always@(posedge clk or posedge rst)begin
    if(rst)begin
      divclk = 26'd0;
    end
    else begin
      divclk = divclk + 26'd1;
    end
  end
endmodule


module division_clock_random (clk, rst, divclk_random);//o
  output reg divclk_random;
  reg [25:0]divclk;
  reg [2:0]random;
  input clk,rst;

  always@(posedge clk or posedge rst)begin
    if(rst)begin
      divclk <= 26'd0;
      divclk_random<=1'b0;
      random<=3'd1;
    end
    else begin
      divclk <= divclk + 26'd1;
      if(random!=3'd0)begin
        random[2]<=random[2]^random[0];
        random[1]<=random[1]^random[2];
        random[0]<=random[0]^random[1];
      end
      else
        random<=3'd1;
      if(divclk[25]==random[2] && divclk[24]==random[1] && divclk[23]==random[0] )
        divclk_random<=~divclk_random;
    end
  end
endmodule

module sec_clock (clk, rst,start_sec_clk, sec_clk);
  reg [24:0]divclk;
  input clk,rst,start_sec_clk;
  output reg sec_clk;
  always@(posedge clk or posedge rst)begin
    if(rst || ~start_sec_clk)begin
      divclk <= 25'd0;
		sec_clk<= 25'd0;
    end
    else begin
      divclk <= divclk +25'd1;
		sec_clk<= ~divclk[24];
    end
  end
endmodule

module fsm (ctrl_return,key_r,key_l,clk,rst,ball,state,R_score,L_score);
  input key_r,key_l;
  input ctrl_return;
  input clk,rst;
  input [9:0]ball;
  output reg [3:0]state;
  input [3:0]R_score,L_score;
  parameter init=4'd0,run_r=4'd1,run_l=4'd2,r_win=4'd3,l_win=4'd4,r_ledx3=4'd5,l_ledx3=4'd6,r_ledx5=4'd7,l_ledx5=4'd8,l_wait=4'd9,r_wait=4'd10;
  always@(posedge clk or posedge rst)
  begin
    if(rst)
	 begin
	   state<=init;
    end
	 else
	 begin
	 case(state)
	   init:begin
		  if(key_l)
		  begin
		    state<=run_r;
		  end
		  else if(key_r)
		  begin
		    state<=run_l;
		  end
		end
		//************
	   run_r:begin
		  if(key_r && ball==10'b10)
		  begin
		    state<=run_l;
		  end
		  else if(ball==10'b1)
		  begin
		    state<=l_win;
		  end
		  else if(key_r && ball!=10'b10)
		  begin
		    state<=l_win;
		  end
		  else 
		  begin
		    state<=run_r;
		  end
		end
	   run_l:begin
		  if(key_l && ball==10'b100000000)
		  begin
		    state<=run_r;
		  end
		  else if(ball==10'b1000000000)
		  begin
		    state<=r_win;
		  end
		  else if(key_l && ball!=10'b100000000)
		  begin
		    state<=r_win;
		  end
		  else 
		  begin
		    state<=run_l;
		  end
		end
		//************
	   r_win:begin
		  if(R_score==4'd6)
		    state<=r_ledx5;
		  else
		    state<=r_ledx3;
		end
	   l_win:begin
		  if(L_score==4'd6)
		    state<=l_ledx5;
		  else
		    state<=l_ledx3;
		end
		//************
		r_ledx3:begin
		  if(ctrl_return)
		    state<=l_wait;
		end
		l_ledx3:begin
		  if(ctrl_return)
		    state<=r_wait;
		end
		//************
		r_ledx5:begin
		  if(ctrl_return)
		    state<=init;
		end
		l_ledx5:begin
		  if(ctrl_return)
		    state<=init;
		end
		//************
		l_wait:begin
		  if(key_r)
		    state<=run_l;
		end
		r_wait:begin
		  if(key_l)
		    state<=run_r;
		end
		//************
	   default:begin
			state<=init;
		end
	 endcase
	 end
  end
endmodule

